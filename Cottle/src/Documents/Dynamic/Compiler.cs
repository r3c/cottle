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

		private readonly ILGenerator generator;

		private readonly Dictionary<Type, Queue<LocalBuilder>> locals;

		private readonly List<string> strings;

		private readonly Dictionary<string, int> stringIndices;

		private readonly Trimmer trimmer;

		private readonly List<Value> values;

		private readonly Dictionary<Value, int> valueIndices;

		#endregion

		#region Constructors

		public Compiler (ILGenerator generator, Trimmer trimmer)
		{
			this.generator = generator;
			this.locals = new Dictionary<Type, Queue<LocalBuilder>> ();
			this.strings = new List<string> ();
			this.stringIndices = new Dictionary<string, int> ();
			this.trimmer = trimmer;
			this.values = new List<Value> ();
			this.valueIndices = new Dictionary<Value, int> ();
		}

		#endregion

		#region Methods / Public

		public Storage Compile (IEnumerable<string> arguments, Command command)
		{
			Label assign;
			Label copy;
			Label exit;
			int index;

			// Create scope for program execution
			this.EmitLoadScope ();
			this.EmitCallStoreEnter ();

			// Assign provided values to arguments
			index = 0;

			foreach (string argument in arguments)
			{
				assign = this.generator.DefineLabel ();
				copy = this.generator.DefineLabel ();

				this.EmitLoadScope ();
				this.EmitLoadValue (argument);

				// Check if a value is available for current argument 
				this.EmitLoadArguments ();

				this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<IList<Value>, int>> ((a) => a.Count).GetGetMethod ());
				this.generator.Emit (OpCodes.Ldc_I4, index);
				this.generator.Emit (OpCodes.Bgt_S, copy);

				// Push void value for current argument
				this.EmitLoadVoid ();

				this.generator.Emit (OpCodes.Br, assign);

				// Fetch argument value from arguments array
				this.generator.MarkLabel (copy);
				this.EmitLoadArguments ();

				this.generator.Emit (OpCodes.Ldc_I4, index);
				this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<IList<Value>, int, Value>> ((a, j) => a[j]));

				// Assign argument value
				this.generator.MarkLabel (assign);

				this.EmitCallStoreSet (StoreMode.Local);

				++index;
			}

			// Compile program body
			exit = this.generator.DefineLabel ();

			this.CompileCommand (command, exit, 0, false);
			this.EmitLoadVoid ();

			this.generator.MarkLabel (exit);

			// Leave scope and return
			this.EmitLoadScope ();
			this.EmitCallStoreLeave ();

			this.generator.Emit (OpCodes.Ret);

			return new Storage (this.strings, this.values);
		}

		#endregion

		#region Methods / Private

		private void CompileCommand (Command command, Label exit, int depth, bool isolate)
		{
			LocalBuilder counter;
			Label empty;
			LocalBuilder enumerator;
			LocalBuilder fields;
			Label jump;
			LocalBuilder operand;
			LocalBuilder pair;
			Label skip;

			// Isolate command scope by entering a new scope level
			if (isolate)
			{
				this.EmitLoadScope ();
				this.EmitCallStoreEnter ();

				++depth;
			}

			// Compile command
			switch (command.Type)
			{
				case CommandType.AssignFunction:
					this.EmitLoadScope ();
					this.EmitLoadValue (command.Name);
					this.EmitLoadValue (new FunctionValue (new Function (command.Arguments, command.Body, this.trimmer, string.Empty)));
					this.EmitCallStoreSet (command.Mode);

					break;

				case CommandType.AssignValue:
					this.CompileExpression (command.Operand);

					operand = this.LocalReserve<Value> ();

					this.generator.Emit (OpCodes.Stloc, operand);

					this.EmitLoadScope ();
					this.EmitLoadValue (command.Name);

					this.generator.Emit (OpCodes.Ldloc, operand);

					this.LocalRelease<Value> (operand);
					this.EmitCallStoreSet (command.Mode);

					break;

				case CommandType.Composite:
					this.CompileCommand (command.Body, exit, depth, false);
					this.CompileCommand (command.Next, exit, depth, false);

					break;

				case CommandType.Dump:
					this.CompileExpression (command.Operand);

					operand = this.LocalReserve<Value> ();

					this.generator.Emit (OpCodes.Stloc, operand);

					this.EmitLoadOutput ();

					this.generator.Emit (OpCodes.Ldloc, operand);
					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<TextWriter, object>> ((w, v) => w.Write (v)));

					this.LocalRelease<Value> (operand);

					break;

				case CommandType.Echo:
					this.CompileExpression (command.Operand);

					operand = this.LocalReserve<Value> ();

					this.generator.Emit (OpCodes.Stloc, operand);

					this.EmitLoadOutput ();

					this.generator.Emit (OpCodes.Ldloc, operand);
					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, string>> ((v) => v.AsString).GetGetMethod ());

					this.LocalRelease<Value> (operand);

					this.EmitCallWriteString ();

					break;

				case CommandType.For:
					empty = this.generator.DefineLabel ();
					jump = this.generator.DefineLabel ();
					skip = this.generator.DefineLabel ();

					// Evaluate operand into fields map
					this.CompileExpression (command.Operand);
					this.EmitCallValueFields ();

					fields = this.LocalReserve<IMap> ();

					this.generator.Emit (OpCodes.Stloc, fields);

					// Get number of fields
					this.generator.Emit (OpCodes.Ldloc, fields);
					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<IMap, int>> ((m) => m.Count).GetGetMethod ());
					this.generator.Emit (OpCodes.Brfalse, empty);

					// Evaluate command for "not empty" case
					this.generator.Emit (OpCodes.Ldloc, fields);
					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<IMap, IEnumerator<KeyValuePair<Value, Value>>>> ((m) => m.GetEnumerator ()));

					this.LocalRelease<IMap> (fields);

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
						this.EmitLoadScope ();
						this.EmitLoadValue (command.Key);

						this.generator.Emit (OpCodes.Ldloca, pair);
						this.generator.Emit (OpCodes.Call, Resolver.Property<Func<KeyValuePair<Value, Value>, Value>> ((p) => p.Key).GetGetMethod ());

						this.EmitCallStoreSet (StoreMode.Local);
					}

					// Store current element value
					this.EmitLoadScope ();
					this.EmitLoadValue (command.Name);

					this.generator.Emit (OpCodes.Ldloca, pair);
					this.generator.Emit (OpCodes.Call, Resolver.Property<Func<KeyValuePair<Value, Value>, Value>> ((p) => p.Value).GetGetMethod ());

					this.LocalRelease<KeyValuePair<Value, Value>> (pair);
					this.EmitCallStoreSet (StoreMode.Local);

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
					this.EmitLoadOutput ();
					this.EmitLoadString (this.trimmer (command.Text));
					this.EmitCallWriteString ();

					break;

				case CommandType.Return:
					this.CompileExpression (command.Operand);

					// Leave all opened scopes if any
					if (depth > 0)
					{
						this.generator.Emit (OpCodes.Ldc_I4, depth);

						counter = this.LocalReserve<int> ();

						this.generator.Emit (OpCodes.Stloc, counter);

						jump = this.generator.DefineLabel ();

						this.generator.MarkLabel (jump);

						this.EmitLoadScope ();
						this.EmitCallStoreLeave ();

						this.generator.Emit (OpCodes.Ldloc, counter);
						this.generator.Emit (OpCodes.Ldc_I4_1);
						this.generator.Emit (OpCodes.Sub);
						this.generator.Emit (OpCodes.Stloc, counter);
						this.generator.Emit (OpCodes.Ldloc, counter);
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

				this.EmitLoadScope ();
				this.EmitCallStoreLeave ();
			}
		}

		private void CompileExpression (Expression expression)
		{
			LocalBuilder arguments;
			ConstructorInfo constructor;
			Label failure;
			LocalBuilder fields;
			LocalBuilder function;
			LocalBuilder key;
			Label success;
			LocalBuilder value;

			switch (expression.Type)
			{
				case ExpressionType.Access:
					success = this.generator.DefineLabel ();

					// Evaluate source expression and get fields
					this.CompileExpression (expression.Source);
					this.EmitCallValueFields ();

					fields = this.LocalReserve<IMap> ();

					this.generator.Emit (OpCodes.Stloc, fields);

					// Evaluate subscript expression
					this.CompileExpression (expression.Subscript);

					value = this.LocalReserve<Value> ();

					this.generator.Emit (OpCodes.Stloc, value);

					// Use subscript to get value from fields
					this.generator.Emit (OpCodes.Ldloc, fields);
					this.generator.Emit (OpCodes.Ldloc, value);
					this.generator.Emit (OpCodes.Ldloca, value);
					this.generator.Emit (OpCodes.Callvirt, typeof (IMap).GetMethod ("TryGet"));
					this.generator.Emit (OpCodes.Brtrue, success);

					// Emit void value on error
					this.EmitLoadVoid ();

					this.generator.Emit (OpCodes.Stloc, value);

					// Push value on stack
					this.generator.MarkLabel (success);
					this.generator.Emit (OpCodes.Ldloc, value);

					this.LocalRelease<Value> (value);

					break;

				case ExpressionType.Constant:
					this.EmitLoadValue (expression.Value);

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
						this.CompileExpression (expression.Arguments[i]);

						value = this.LocalReserve<Value> ();

						this.generator.Emit (OpCodes.Stloc, value);
						this.generator.Emit (OpCodes.Ldloc, arguments);
						this.generator.Emit (OpCodes.Ldc_I4, i);
						this.generator.Emit (OpCodes.Ldloc, value);
						this.generator.Emit (OpCodes.Stelem_Ref);

						this.LocalRelease<Value> (value);
					}

					// Invoke function delegate within exception block
					this.generator.Emit (OpCodes.Ldloc, function);
					this.generator.Emit (OpCodes.Ldloc, arguments);

					this.LocalRelease<Value[]> (arguments);
					this.LocalRelease<IFunction> (function);
					this.EmitLoadScope ();
					this.EmitLoadOutput ();
					this.EmitCallFunctionExecute ();

					value = this.LocalReserve<Value> ();

					this.generator.Emit (OpCodes.Stloc, value);
					this.generator.Emit (OpCodes.Br_S, success);

					// Emit void value on error
					this.generator.MarkLabel (failure);

					this.EmitLoadVoid ();

					this.generator.Emit (OpCodes.Stloc, value);

					// Value is already available on stack
					this.generator.MarkLabel (success);
					this.generator.Emit (OpCodes.Ldloc, value);

					this.LocalRelease<Value> (value);

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
						this.CompileExpression (expression.Elements[i].Key);

						key = this.LocalReserve<Value> ();

						this.generator.Emit (OpCodes.Stloc, key);

						this.CompileExpression (expression.Elements[i].Value);

						value = this.LocalReserve<Value> ();

						this.generator.Emit (OpCodes.Stloc, value);
						this.generator.Emit (OpCodes.Ldloc, arguments);
						this.generator.Emit (OpCodes.Ldc_I4, i);
						this.generator.Emit (OpCodes.Ldelema, typeof (KeyValuePair<Value, Value>));
						this.generator.Emit (OpCodes.Ldloc, key);
						this.generator.Emit (OpCodes.Ldloc, value);
						this.generator.Emit (OpCodes.Newobj, constructor);
						this.generator.Emit (OpCodes.Stobj, typeof (KeyValuePair<Value, Value>));

						this.LocalRelease<Value> (key);
						this.LocalRelease<Value> (value);
					}

					// Create value from array
					constructor = Resolver.Constructor<Func<IEnumerable<KeyValuePair<Value, Value>>, Value>> ((f) => new MapValue (f));

					this.generator.Emit (OpCodes.Ldloc, arguments);
					this.generator.Emit (OpCodes.Newobj, constructor);

					this.LocalRelease<KeyValuePair<Value, Value>[]> (arguments);

					break;

				case ExpressionType.Symbol:
					success = this.generator.DefineLabel ();

					// Get variable from scope
					this.EmitLoadScope ();
					this.EmitLoadValue (expression.Value);

					value = this.LocalReserve<Value> ();

					this.generator.Emit (OpCodes.Ldloca, value);
					this.generator.Emit (OpCodes.Callvirt, typeof (IStore).GetMethod ("TryGet"));
					this.generator.Emit (OpCodes.Brtrue, success);

					// Emit void value on error
					this.EmitLoadVoid ();

					this.generator.Emit (OpCodes.Stloc, value);

					// Push value on stack
					this.generator.MarkLabel (success);
					this.generator.Emit (OpCodes.Ldloc, value);

					this.LocalRelease<Value> (value);

					break;

				case ExpressionType.Void:
					this.EmitLoadVoid ();

					break;
			}
		}

		private void EmitCallFunctionExecute ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<IFunction, IList<Value>, IStore, TextWriter, Value>> ((f, a, s, o) => f.Execute (a, s, o)));
		}

		private void EmitCallStoreEnter ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IStore>> ((s) => s.Enter ()));
		}

		private void EmitCallStoreLeave ()
		{
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IStore>> ((s) => s.Leave ()));
			this.generator.Emit (OpCodes.Pop);
		}

		private void EmitCallStoreSet (StoreMode mode)
		{
			this.generator.Emit (OpCodes.Ldc_I4, (int)mode);
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IStore, Value, Value, StoreMode>> ((s, n, v, m) => s.Set (n, v, m)));
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

		private void EmitLoadArguments ()
		{
			this.generator.Emit (OpCodes.Ldarg_1);
		}

		private void EmitLoadContext ()
		{
			this.generator.Emit (OpCodes.Ldarga, 0);
		}

		private void EmitLoadOutput ()
		{
			this.generator.Emit (OpCodes.Ldarg_3);
		}

		private void EmitLoadScope ()
		{
			this.generator.Emit (OpCodes.Ldarg_2);
		}

		private void EmitLoadString (string literal)
		{
			int index;

			if (!this.stringIndices.TryGetValue (literal, out index))
			{
				index = this.strings.Count;

				this.stringIndices[literal] = index;
				this.strings.Add (literal);
			}

			this.EmitLoadContext ();

			this.generator.Emit (OpCodes.Ldfld, Resolver.Field<Func<Storage, string[]>> ((c) => c.Strings));
			this.generator.Emit (OpCodes.Ldc_I4, index);
			this.generator.Emit (OpCodes.Ldelem_Ref);
		}

		private void EmitLoadValue (Value constant)
		{
			int index;

			if (!this.valueIndices.TryGetValue (constant, out index))
			{
				index = this.values.Count;

				this.valueIndices[constant] = index;
				this.values.Add (constant);
			}

			this.EmitLoadContext ();

			this.generator.Emit (OpCodes.Ldfld, Resolver.Field<Func<Storage, Value[]>> ((c) => c.Values));
			this.generator.Emit (OpCodes.Ldc_I4, index);
			this.generator.Emit (OpCodes.Ldelem_Ref);
		}

		private void EmitLoadVoid ()
		{
			this.generator.Emit (OpCodes.Call, Resolver.Property<Func<Value>> (() => VoidValue.Instance).GetGetMethod ());
		}

		private void LocalRelease<T> (LocalBuilder local)
		{
			Queue<LocalBuilder> queue;

			if (!this.locals.TryGetValue (typeof (T), out queue))
			{
				queue = new Queue<LocalBuilder> ();

				this.locals[typeof (T)] = queue;
			}

			queue.Enqueue (local);
		}

		private LocalBuilder LocalReserve<T> ()
		{
			Queue<LocalBuilder> queue;

			if (this.locals.TryGetValue (typeof (T), out queue) && queue.Count > 0)
				return queue.Dequeue ();

			return this.generator.DeclareLocal (typeof (T));
		}

		#endregion
	}
}
