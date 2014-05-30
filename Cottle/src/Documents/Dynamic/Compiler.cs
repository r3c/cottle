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

		private readonly ILGenerator							generator;

		private readonly Dictionary<Type, Queue<LocalBuilder>>	locals;

		private readonly List<string>							strings;

		private readonly Trimmer								trimmer;

		private readonly List<Value>							values;

		#endregion

		#region Constructors

		public Compiler (ILGenerator generator, Trimmer trimmer)
		{
			this.generator = generator;
			this.locals = new Dictionary<Type, Queue<LocalBuilder>> ();
			this.strings = new List<string> ();
			this.trimmer = trimmer;
			this.values = new List<Value> ();
		}

		#endregion

		#region Methods / Public

		public Storage CompileFunction (IEnumerable<string> arguments, Command command)
		{
			Label	assign;
			Label	copy;
			Label	exit;
			int		index;

			this.EmitPushScope ();
			this.EmitCallScopeEnter ();

			index = 0;

			foreach (string argument in arguments)
			{
				assign = this.generator.DefineLabel ();
				copy = this.generator.DefineLabel ();

				this.EmitPushScope ();
				this.EmitPushValue (argument);

				// Check if a value is available for current argument 
				this.EmitPushArguments ();

				this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<IList<Value>, int>> ((a) => a.Count).GetGetMethod ());
				this.generator.Emit (OpCodes.Ldc_I4, index);
				this.generator.Emit (OpCodes.Bgt_S, copy);

				// Push void value for current argument
				this.EmitPushVoid ();

				this.generator.Emit (OpCodes.Br, assign);

				// Fetch argument value from arguments array
				this.generator.MarkLabel (copy);
				this.EmitPushArguments ();

				this.generator.Emit (OpCodes.Ldc_I4, index);
				this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<IList<Value>, int, Value>> ((a, j) => a[j]));

				// Assign argument value
				this.generator.MarkLabel (assign);

				this.EmitCallScopeSet (ScopeMode.Local);

				++index;
			}

			exit = this.generator.DefineLabel ();

			this.CompileCommand (command, exit, 0, false);
			this.EmitPushVoid ();

			this.generator.MarkLabel (exit);

			this.EmitPushScope ();
			this.EmitCallScopeLeave ();

			this.generator.Emit (OpCodes.Ret);

			return new Storage (this.strings, this.values);
		}

		public Storage CompileProgram (Command command)
		{
			Label	exit;

			exit = this.generator.DefineLabel ();

			this.CompileCommand (command, exit, 0, false);
			this.EmitPushVoid ();

			this.generator.MarkLabel (exit);
			this.generator.Emit (OpCodes.Ret);

			return new Storage (this.strings, this.values);
		}

		#endregion

		#region Methods / Private

		private void CompileCommand (Command command, Label exit, int depth, bool isolate)
		{
			LocalBuilder	counter;
			Label			empty;
			LocalBuilder	enumerator;
			Label			jump;
			LocalBuilder	map;
			LocalBuilder	pair;
			Label			skip;

			// Isolate command scope by entering a new scope level
			if (isolate)
			{
				this.EmitPushScope ();
				this.EmitCallScopeEnter ();

				++depth;
			}

			// Compile command
			switch (command.Type)
			{
				case CommandType.AssignFunction:
					this.EmitPushScope ();
					this.EmitPushValue (command.Name);
					this.EmitPushValue (new FunctionValue (new Function (command.Arguments, command.Body, this.trimmer)));
					this.EmitCallScopeSet (command.Mode);

					break;

				case CommandType.AssignValue:
					this.EmitPushScope ();
					this.EmitPushValue (command.Name);
					this.CompileExpression (command.Operand);
					this.EmitCallScopeSet (command.Mode);

					break;

				case CommandType.Composite:
					for (; command != null && command.Type == CommandType.Composite; command = command.Next)
						this.CompileCommand (command.Body, exit, depth, false);

					if (command != null)
						this.CompileCommand (command, exit, depth, false);

					break;

				case CommandType.Dump:
					this.EmitPushOutput ();
					this.CompileExpression (command.Operand);

					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<TextWriter, object>> ((w, v) => w.Write (v)));

					break;

				case CommandType.Echo:
					this.EmitPushOutput ();
					this.CompileExpression (command.Operand);

					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, string>> ((v) => v.AsString).GetGetMethod ());

					this.EmitCallWriteString ();

					break;

				case CommandType.For:
					empty = this.generator.DefineLabel ();
					jump = this.generator.DefineLabel ();
					skip = this.generator.DefineLabel ();

					// Evaluate operand into fields map
					this.CompileExpression (command.Operand);
					this.EmitCallValueFields ();

					map = this.LocalReserve<IMap> ();

					this.generator.Emit (OpCodes.Stloc, map);

					// Get number of fields
					this.generator.Emit (OpCodes.Ldloc, map);
					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<IMap, int>> ((m) => m.Count).GetGetMethod ());
					this.generator.Emit (OpCodes.Brfalse, empty);

					// Evaluate command for "not empty" case
					this.generator.Emit (OpCodes.Ldloc, map);

					this.LocalRelease<IMap> (map);

					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<IMap, IEnumerator<KeyValuePair<Value, Value>>>> ((m) => m.GetEnumerator ()));

					enumerator = this.LocalReserve<IEnumerator<KeyValuePair<Value, Value>>> ();

					this.generator.Emit (OpCodes.Stloc, enumerator);

					// Fetch next enumerator element or end loop
					this.generator.MarkLabel (jump);
					this.generator.Emit (OpCodes.Ldloc, enumerator);
					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<IEnumerator<KeyValuePair<Value, Value>>, bool>> ((e) => e.MoveNext ()));
					this.generator.Emit (OpCodes.Brfalse, skip);
					this.generator.Emit (OpCodes.Ldloc, enumerator);

					this.LocalRelease<IEnumerator<KeyValuePair<Value, Value>>> (enumerator);

					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<IEnumerator<KeyValuePair<Value, Value>>, KeyValuePair<Value, Value>>> ((e) => e.Current).GetGetMethod ());

					pair = this.LocalReserve<KeyValuePair<Value, Value>> ();

					this.generator.Emit (OpCodes.Stloc, pair);

					// Store current element key if required
					if (!string.IsNullOrEmpty (command.Key))
					{
						this.EmitPushScope ();
						this.EmitPushValue (command.Key);

						this.generator.Emit (OpCodes.Ldloca, pair);
						this.generator.Emit (OpCodes.Call, Resolver.Property<Func<KeyValuePair<Value, Value>, Value>> ((p) => p.Key).GetGetMethod ());

						this.EmitCallScopeSet (ScopeMode.Local);
					}

					// Store current element value
					this.EmitPushScope ();
					this.EmitPushValue (command.Name);

					this.generator.Emit (OpCodes.Ldloca, pair);

					this.LocalRelease<KeyValuePair<Value, Value>> (pair);

					this.generator.Emit (OpCodes.Call, Resolver.Property<Func<KeyValuePair<Value, Value>, Value>> ((p) => p.Value).GetGetMethod ());

					this.EmitCallScopeSet (ScopeMode.Local);

					// Evaluate body and restart cycle
					this.CompileCommand (command.Body, exit, depth, true);

					this.generator.Emit (OpCodes.Br, jump);

					// Evaluate command for "empty" case
					this.generator.MarkLabel (empty);

					if (command.Next != null)
						this.CompileCommand (command.Next, exit, depth, true);

					// Mark end of statement
					this.generator.MarkLabel (skip);

					break;

				case CommandType.If:
					skip = this.generator.DefineLabel ();

					// Emit conditional branches
					for (; command != null && command.Type == CommandType.If; command = command.Next)
					{
						jump = this.generator.DefineLabel ();

						// Evaluate branch condition, jump to next if false
						this.CompileExpression (command.Operand);
						this.EmitCallValueAsBoolean ();

						this.generator.Emit (OpCodes.Brfalse, jump);

						// Execute branch command and jump sibling statements
						this.CompileCommand (command.Body, exit, depth, true);

						this.generator.Emit (OpCodes.Br, skip);
						this.generator.MarkLabel (jump);
					}

					// Emit fallback branch if any
					if (command != null)
						this.CompileCommand (command, exit, depth, true);

					// Mark end of statement
					this.generator.MarkLabel (skip);

					break;

				case CommandType.Literal:
					this.EmitPushOutput ();
					this.EmitPushString (this.trimmer (command.Text));
					this.EmitCallWriteString ();

					break;

				case CommandType.Return:
					this.CompileExpression (command.Operand);

					// Leave all opened scopes if any
					if (depth > 0)
					{
						counter = this.LocalReserve<int> ();
						jump = this.generator.DefineLabel ();

						this.generator.Emit (OpCodes.Ldc_I4, depth);
						this.generator.Emit (OpCodes.Stloc);
						this.generator.MarkLabel (jump);

						this.EmitPushScope ();
						this.EmitCallScopeLeave ();

						this.generator.Emit (OpCodes.Ldloc, depth);
						this.generator.Emit (OpCodes.Ldc_I4_1);
						this.generator.Emit (OpCodes.Sub);
						this.generator.Emit (OpCodes.Stloc, depth);
						this.generator.Emit (OpCodes.Ldloc, depth);
						this.generator.Emit (OpCodes.Brtrue, jump);

						this.LocalRelease<int> (counter);
					}

					this.generator.Emit (OpCodes.Br, exit);

					break;

				case CommandType.While:
					jump = this.generator.DefineLabel ();
					skip = this.generator.DefineLabel ();

					// Branch to condition before first body execution
					this.generator.Emit (OpCodes.Br, skip);

					// Execute loop command
					this.generator.MarkLabel (jump);

					this.CompileCommand (command.Body, exit, depth, true);

					// Evaluate loop condition, restart cycle if true
					this.generator.MarkLabel (skip);

					this.CompileExpression (command.Operand);
					this.EmitCallValueAsBoolean ();

					this.generator.Emit (OpCodes.Brtrue, jump);

					break;
			}

			// Cancel command isolation by leaving scope level
			if (isolate)
			{
				--depth;

				this.EmitPushScope ();
				this.EmitCallScopeLeave ();
			}
		}

		private void CompileExpression (Expression expression)
		{
			LocalBuilder	arguments;
			ConstructorInfo	constructor;
			Label			failure;
			LocalBuilder	function;
			Label			success;
			LocalBuilder	value;

			switch (expression.Type)
			{
				case ExpressionType.Access:
					success = this.generator.DefineLabel ();

					// Evaluate source expression and get fields
					this.CompileExpression (expression.Source);
					this.EmitCallValueFields ();

					// Evaluate subscript expression
					this.CompileExpression (expression.Subscript);

					// Use subscript to get value from fields
					value = this.LocalReserve<Value> ();

					this.generator.Emit (OpCodes.Ldloca, value);
					this.generator.Emit (OpCodes.Callvirt, typeof (IMap).GetMethod ("TryGet"));
					this.generator.Emit (OpCodes.Brtrue, success);

					// Emit void value on error
					this.EmitPushVoid ();

					this.generator.Emit (OpCodes.Stloc, value);

					// Push value on stack
					this.generator.MarkLabel (success);
					this.generator.Emit (OpCodes.Ldloc, value);

					this.LocalRelease<Value> (value);

					break;

				case ExpressionType.Constant:
					this.EmitPushValue (expression.Value);

					break;

				case ExpressionType.Invoke:
					failure = this.generator.DefineLabel ();
					success = this.generator.DefineLabel ();

					// Evaluate source expression as a function
					this.CompileExpression (expression.Source);

					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, IFunction>> ((v) => v.AsFunction).GetGetMethod ());

					function = this.LocalReserve<IFunction> ();

					this.generator.Emit (OpCodes.Stloc, function);
					this.generator.Emit (OpCodes.Ldloc, function);
					this.generator.Emit (OpCodes.Brfalse, failure);

					// Create array to store evaluated values 
					this.generator.Emit (OpCodes.Ldc_I4, expression.Arguments.Length);
					this.generator.Emit (OpCodes.Newarr, typeof (Value));

					arguments = this.LocalReserve<Value[]> ();

					this.generator.Emit (OpCodes.Stloc, arguments);

					// Evaluate arguments and store into array
					for (int i = 0; i < expression.Arguments.Length; ++i)
					{
						this.generator.Emit (OpCodes.Ldloc, arguments);
						this.generator.Emit (OpCodes.Ldc_I4, i);

						this.CompileExpression (expression.Arguments[i]);

						this.generator.Emit (OpCodes.Stelem_Ref);
					}

					// Invoke function delegate within exception block
					this.generator.Emit (OpCodes.Ldloc, function);

					this.LocalRelease<IFunction> (function);

					this.generator.Emit (OpCodes.Ldloc, arguments);

					this.LocalRelease<Value[]> (arguments);

					this.EmitPushScope ();
					this.EmitPushOutput ();

					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<IFunction, IList<Value>, IScope, TextWriter, Value>> ((f, a, s, o) => f.Execute (a, s, o)));
					this.generator.Emit (OpCodes.Br_S, success);

					// Emit void value on error
					this.generator.MarkLabel (failure);

					this.EmitPushVoid ();

					// Value is already available on stack
					this.generator.MarkLabel (success);

					break;

				case ExpressionType.Map:
					// Create array to store evaluated pairs
					this.generator.Emit (OpCodes.Ldc_I4, expression.Elements.Length);
					this.generator.Emit (OpCodes.Newarr, typeof (KeyValuePair<Value, Value>));

					arguments = this.LocalReserve<KeyValuePair<Value, Value>[]> ();

					this.generator.Emit (OpCodes.Stloc, arguments);

					// Evaluate elements and store into array 
					constructor = Resolver.Constructor<Func<Value, Value, KeyValuePair<Value, Value>>> ((k, v) => new KeyValuePair<Value, Value> (k, v));

					for (int i = 0; i < expression.Elements.Length; ++i)
					{
						this.generator.Emit (OpCodes.Ldloc, arguments);
						this.generator.Emit (OpCodes.Ldc_I4, i);
						this.generator.Emit (OpCodes.Ldelema, typeof (KeyValuePair<Value, Value>));

						this.CompileExpression (expression.Elements[i].Key);
						this.CompileExpression (expression.Elements[i].Value);

						this.generator.Emit (OpCodes.Newobj, constructor);
						this.generator.Emit (OpCodes.Stobj, typeof (KeyValuePair<Value, Value>));
					}

					// Create value from array
					constructor = Resolver.Constructor<Func<IEnumerable<KeyValuePair<Value, Value>>, Value>> ((f) => new MapValue (f));

					this.generator.Emit (OpCodes.Ldloc, arguments);

					this.LocalRelease<KeyValuePair<Value, Value>[]> (arguments);

					this.generator.Emit (OpCodes.Newobj, constructor);

					break;

				case ExpressionType.Symbol:
					success = this.generator.DefineLabel ();

					// Get variable from scope
					this.EmitPushScope ();
					this.EmitPushValue (expression.Value);

					value = this.LocalReserve<Value> ();

					this.generator.Emit (OpCodes.Ldloca, value);
					this.generator.Emit (OpCodes.Callvirt, typeof (IScope).GetMethod ("Get"));
					this.generator.Emit (OpCodes.Brtrue, success);

					// Emit void value on error
					this.EmitPushVoid ();

					this.generator.Emit (OpCodes.Stloc, value);

					// Push value on stack
					this.generator.MarkLabel (success);
					this.generator.Emit (OpCodes.Ldloc, value);

					this.LocalRelease<Value> (value);

					break;

				case ExpressionType.Void:
					this.EmitPushVoid ();

					break;
			}
		}

		private void EmitCallScopeEnter ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IScope>> ((s) => s.Enter ()));
		}

		private void EmitCallScopeLeave ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IScope>> ((s) => s.Leave ()));
			this.generator.Emit (OpCodes.Pop);
		}

		private void EmitCallScopeSet (ScopeMode mode)
		{
			this.generator.Emit (OpCodes.Ldc_I4, (int)mode);
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IScope, Value, Value, ScopeMode>> ((s, n, v, m) => s.Set (n, v, m)));
		}

		private void EmitCallValueAsBoolean ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, bool>> ((v) => v.AsBoolean).GetGetMethod ());
		}

		private void EmitCallValueFields ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, IMap>> ((v) => v.Fields).GetGetMethod ());
		}

		private void EmitCallWriteString ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<TextWriter, string>> ((w, v) => w.Write (v)));
		}

		private void EmitPushArguments ()
		{
			this.generator.Emit (OpCodes.Ldarg_1);
		}

		private void EmitPushContext ()
		{
			this.generator.Emit (OpCodes.Ldarga, 0);
		}

		private void EmitPushOutput ()
		{
			this.generator.Emit (OpCodes.Ldarg_3);
		}

		private void EmitPushScope ()
		{
			this.generator.Emit (OpCodes.Ldarg_2);
		}

		private void EmitPushString (string literal)
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

			this.generator.Emit (OpCodes.Ldfld, Resolver.Field<Func<Storage, string[]>> ((c) => c.Strings));
			this.generator.Emit (OpCodes.Ldc_I4, index);
			this.generator.Emit (OpCodes.Ldelem_Ref);
		}

		private void EmitPushValue (Value constant)
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

			this.generator.Emit (OpCodes.Ldfld, Resolver.Field<Func<Storage, Value[]>> ((c) => c.Values));
			this.generator.Emit (OpCodes.Ldc_I4, index);
			this.generator.Emit (OpCodes.Ldelem_Ref);
		}

		private void EmitPushVoid ()
		{
			this.generator.Emit (OpCodes.Call, Resolver.Property<Func<Value>> (() => VoidValue.Instance).GetGetMethod ());
		}

		private void LocalRelease<T> (LocalBuilder local)
		{
			Queue<LocalBuilder>	queue;

			if (!this.locals.TryGetValue (typeof (T), out queue))
			{
				queue = new Queue<LocalBuilder> ();

				this.locals[typeof (T)] = queue;
			}

			queue.Enqueue (local);
		}

		private LocalBuilder LocalReserve<T> ()
		{
			Queue<LocalBuilder>	queue;

			if (this.locals.TryGetValue (typeof (T), out queue) && queue.Count > 0)
				return queue.Dequeue ();

			return this.generator.DeclareLocal (typeof (T));
		}

		#endregion
	}
}
