using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Cottle.Values;

namespace Cottle.Documents.Dynamic
{
	class Compiler
	{
		#region Attributes

		private readonly ILGenerator	generator;

		private LocalBuilder			localFunction;

		private LocalBuilder			localValue;

		private readonly Label			terminate;

		private readonly Trimmer		trimmer;

		private readonly List<string>	strings;

		private readonly List<Value>	values;

		#endregion

		#region Constructors

		public Compiler (ILGenerator generator, Trimmer trimmer)
		{
			this.generator = generator;
			this.localFunction = null;
			this.localValue = null;
			this.terminate = generator.DefineLabel ();
			this.trimmer = trimmer;
			this.strings = new List<string> ();
			this.values = new List<Value> ();
		}

		#endregion

		#region Methods / Public

		public Storage Compile (Command command)
		{
			this.CompileCommand (command);
			this.EmitVoid ();

			this.generator.MarkLabel (this.terminate);
			this.generator.Emit (OpCodes.Ret);

			return new Storage (this.strings, this.values);
		}

		#endregion

		#region Methods / Private

		private void CompileCommand (Command command)
		{
			Label	jump;
			Label	tail;

			switch (command.Type)
			{
				case CommandType.AssignFunction:
					throw new NotImplementedException ();

				case CommandType.AssignValue:
					this.EmitPushScope ();
					this.EmitValue (command.Name);
					this.CompileExpression (command.Operand);

					this.generator.Emit (OpCodes.Ldc_I4, (int)command.Mode);
					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IScope, Value, Value, ScopeMode>> ((scope, symbol, value, mode) => scope.Set (symbol, value, mode)));

					break;

				case CommandType.Composite:
					for (; command != null && command.Type == CommandType.Composite; command = command.Next)
						this.CompileCommand (command.Body);

					if (command != null)
						this.CompileCommand (command);

					break;

				case CommandType.Dump:
					this.EmitPushOutput ();
					this.CompileExpression (command.Operand);

					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<TextWriter, object>> ((writer, value) => writer.Write (value)));

					break;

				case CommandType.Echo:
					this.EmitPushOutput ();
					this.CompileExpression (command.Operand);

					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, string>> ((value) => value.AsString).GetGetMethod ());

					this.EmitCallWriteString ();

					break;

				case CommandType.For:
					throw new NotImplementedException ();

				case CommandType.If:
					tail = this.generator.DefineLabel ();

					// Emit conditional branches
					for (; command != null && command.Type == CommandType.If; command = command.Next)
					{
						jump = this.generator.DefineLabel ();

						// Evaluate branch condition, jump to next if false
						this.CompileExpression (command.Operand);
						this.EmitCallValueAsBoolean ();

						this.generator.Emit (OpCodes.Brfalse, jump);

						// Execute branch command and exit statement
						this.CompileCommand (command.Body);

						this.generator.Emit (OpCodes.Br, tail);
						this.generator.MarkLabel (jump);
					}

					// Emit fallback branch if any
					if (command != null)
						this.CompileCommand (command);

					// Mark end of statement
					this.generator.MarkLabel (tail);

					break;

				case CommandType.Literal:
					this.EmitPushOutput ();
					this.EmitString (this.trimmer (command.Text));
					this.EmitCallWriteString ();

					break;

				case CommandType.Return:
					this.CompileExpression (command.Operand);

					this.generator.Emit (OpCodes.Br, this.terminate);

					break;

				case CommandType.While:
					jump = this.generator.DefineLabel ();
					tail = this.generator.DefineLabel ();

					// Branch to condition before first body execution
					this.generator.Emit (OpCodes.Br, tail);

					// Execute loop command
					this.generator.MarkLabel (jump);

					this.CompileCommand (command.Body);

					// Evaluate loop condition, restart cycle if true
					this.generator.MarkLabel (tail);

					this.CompileExpression (command.Operand);
					this.EmitCallValueAsBoolean ();

					this.generator.Emit (OpCodes.Brtrue, jump);

					break;
			}
		}

		private void CompileExpression (Expression expression)
		{
			ConstructorInfo	constructor;
			Label			failure;
			LocalBuilder	localArguments;
			LocalBuilder	localCaller;
			LocalBuilder	localError;
			Label			success;

			switch (expression.Type)
			{
				case ExpressionType.Access:
					success = this.generator.DefineLabel ();

					// Evaluate source expression and get fields
					this.CompileExpression (expression.Source);

					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, IMap>> ((value) => value.Fields).GetGetMethod ());

					// Evaluate subscript expression
					this.CompileExpression (expression.Subscript);

					// Use subscript to get value from fields
					this.generator.Emit (OpCodes.Ldloca, this.DefineLocalValue ());
					this.generator.Emit (OpCodes.Callvirt, typeof (IMap).GetMethod ("TryGet"));
					this.generator.Emit (OpCodes.Brtrue, success);

					// Emit void value on error
					this.EmitVoid ();

					this.generator.Emit (OpCodes.Stloc, this.DefineLocalValue ());

					// Push value on stack
					this.generator.MarkLabel (success);
					this.generator.Emit (OpCodes.Ldloc, this.DefineLocalValue ());

					break;

				case ExpressionType.Constant:
					this.EmitValue (expression.Value);

					break;

				case ExpressionType.Invoke:
					localArguments = this.generator.DeclareLocal (typeof (Value[]));
					localCaller = this.generator.DeclareLocal (typeof (Value));
					localError = this.generator.DeclareLocal (typeof (Exception));
					failure = this.generator.DefineLabel ();
					success = this.generator.DefineLabel ();

					// Evaluate source expression as a function
					this.CompileExpression (expression.Source);

					this.generator.Emit (OpCodes.Stloc, localCaller);
					this.generator.Emit (OpCodes.Ldloc, localCaller);
					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, IFunction>> ((value) => value.AsFunction).GetGetMethod ());
					this.generator.Emit (OpCodes.Stloc, this.DefineLocalFunction ());
					this.generator.Emit (OpCodes.Ldloc, this.DefineLocalFunction ());
					this.generator.Emit (OpCodes.Brfalse, failure);

					// Create array to store evaluated values 
					this.generator.Emit (OpCodes.Ldc_I4, expression.Arguments.Length);
					this.generator.Emit (OpCodes.Newarr, typeof (Value));
					this.generator.Emit (OpCodes.Stloc, localArguments);

					// Evaluate arguments and store into array
					for (int i = 0; i < expression.Arguments.Length; ++i)
					{
						this.generator.Emit (OpCodes.Ldloc, localArguments);
						this.generator.Emit (OpCodes.Ldc_I4, i);

						this.CompileExpression (expression.Arguments[i]);

						this.generator.Emit (OpCodes.Stelem_Ref);
					}

					// Invoke function delegate within exception block
					this.generator.Emit (OpCodes.Ldloc, this.DefineLocalFunction ());
					this.generator.Emit (OpCodes.Ldloc, localArguments);

					this.EmitPushScope ();
					this.EmitPushOutput ();

					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<IFunction, IList<Value>, IScope, TextWriter, Value>> ((function, arguments, scope, output) => function.Execute (arguments, scope, output)));
					this.generator.Emit (OpCodes.Br_S, success);

					// Emit void value on error
					this.generator.MarkLabel (failure);

					this.EmitVoid ();

					// Value is already available on stack
					this.generator.MarkLabel (success);

					break;

				case ExpressionType.Map:
					localArguments = this.generator.DeclareLocal (typeof (KeyValuePair<Value, Value>[]));

					// Create array to store evaluated pairs
					this.generator.Emit (OpCodes.Ldc_I4, expression.Elements.Length);
					this.generator.Emit (OpCodes.Newarr, typeof (KeyValuePair<Value, Value>));
					this.generator.Emit (OpCodes.Stloc, localArguments);

					// Evaluate elements and store into array 
					constructor = Resolver.Constructor<Func<Value, Value, KeyValuePair<Value, Value>>> ((key, value) => new KeyValuePair<Value, Value> (key, value));

					for (int i = 0; i < expression.Elements.Length; ++i)
					{
						this.generator.Emit (OpCodes.Ldloc, localArguments);
						this.generator.Emit (OpCodes.Ldc_I4, i);
						this.generator.Emit (OpCodes.Ldelema, typeof (KeyValuePair<Value, Value>));

						this.CompileExpression (expression.Elements[i].Key);
						this.CompileExpression (expression.Elements[i].Value);

						this.generator.Emit (OpCodes.Newobj, constructor);
						this.generator.Emit (OpCodes.Stobj, typeof (KeyValuePair<Value, Value>));
					}

					// Create value from array
					constructor = Resolver.Constructor<Func<IEnumerable<KeyValuePair<Value, Value>>, Value>> ((pairs) => new MapValue (pairs));

					this.generator.Emit (OpCodes.Ldloc, localArguments);
					this.generator.Emit (OpCodes.Newobj, constructor);

					break;

				case ExpressionType.Symbol:
					success = this.generator.DefineLabel ();

					// Get variable from scope
					this.EmitPushScope ();
					this.EmitValue (expression.Value);

					this.generator.Emit (OpCodes.Ldloca, this.DefineLocalValue ());
					this.generator.Emit (OpCodes.Callvirt, typeof (IScope).GetMethod ("Get"));
					this.generator.Emit (OpCodes.Brtrue, success);

					// Emit void value on error
					this.EmitVoid ();

					this.generator.Emit (OpCodes.Stloc, this.DefineLocalValue ());

					// Push value on stack
					this.generator.MarkLabel (success);
					this.generator.Emit (OpCodes.Ldloc, this.DefineLocalValue ());

					break;

				case ExpressionType.Void:
					this.EmitVoid ();

					break;
			}
		}

		private LocalBuilder DefineLocalFunction ()
		{
			if (this.localFunction == null)
				this.localFunction = this.generator.DeclareLocal (typeof (IFunction));

			return this.localFunction;
		}

		private LocalBuilder DefineLocalValue ()
		{
			if (this.localValue == null)
				this.localValue = this.generator.DeclareLocal (typeof (Value));

			return this.localValue;
		}

		private void EmitCallValueAsBoolean ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, bool>> ((value) => value.AsBoolean).GetGetMethod ());
		}

		private void EmitCallWriteString ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<TextWriter, string>> ((writer, value) => writer.Write (value)));
		}

		private void EmitPushContext ()
		{
			this.generator.Emit (OpCodes.Ldarga, 0);
		}

		private void EmitPushOutput ()
		{
			this.generator.Emit (OpCodes.Ldarg_2);
		}

		private void EmitPushScope ()
		{
			this.generator.Emit (OpCodes.Ldarg_1);
		}

		private void EmitString (string literal)
		{
			int	index;

			// FIXME: slow
			index = this.strings.IndexOf (literal);

			if (index < 0)
			{
				index = this.strings.Count;

				this.strings.Add (literal);
			}

			this.EmitPushContext ();

			this.generator.Emit (OpCodes.Ldfld, Resolver.Field<Func<Storage, string[]>> ((context) => context.Strings));
			this.generator.Emit (OpCodes.Ldc_I4, index);
			this.generator.Emit (OpCodes.Ldelem_Ref);
		}

		private void EmitValue (Value constant)
		{
			int	index;

			// FIXME: slow
			index = this.values.IndexOf (constant);

			if (index < 0)
			{
				index = this.values.Count;

				this.values.Add (constant);
			}

			this.EmitPushContext ();

			this.generator.Emit (OpCodes.Ldfld, Resolver.Field<Func<Storage, Value[]>> ((context) => context.Values));
			this.generator.Emit (OpCodes.Ldc_I4, index);
			this.generator.Emit (OpCodes.Ldelem_Ref);
		}

		private void EmitVoid ()
		{
			this.generator.Emit (OpCodes.Call, Resolver.Property<Func<Value>> (() => VoidValue.Instance).GetGetMethod ());
		}

		#endregion
	}
}
