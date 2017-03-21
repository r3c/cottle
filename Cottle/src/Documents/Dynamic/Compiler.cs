using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Cottle.Values;

namespace Cottle.Documents.Dynamic
{
	class Compiler
	{
		#region Attributes

		private readonly List<Value> constants;

		private readonly ILGenerator generator;

		private readonly Dictionary<Value, int> indices;

		private readonly Dictionary<Type, Queue<LocalBuilder>> locals;

		private readonly Trimmer trimmer;

		#endregion

		#region Constructors

		public Compiler (ILGenerator generator, Trimmer trimmer)
		{
			this.constants = new List<Value> ();
			this.generator = generator;
			this.indices = new Dictionary<Value, int> ();
			this.locals = new Dictionary<Type, Queue<LocalBuilder>> ();
			this.trimmer = trimmer;
		}

		#endregion

		#region Methods / Public

		public Storage Compile (IEnumerable<string> arguments, Command command)
		{
			Label assign;
			Label copy;
			Label exit;
			int index;

			// Create global scope for program execution
			this.EmitStoreEnter ();

			// Assign provided values to arguments
			index = 0;

			foreach (string argument in arguments)
			{
				assign = this.generator.DefineLabel ();
				copy = this.generator.DefineLabel ();

				this.EmitLoadStore ();
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
				this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<IList<Value>, int, Value>> ((a, i) => a[i]));

				// Assign argument value
				this.generator.MarkLabel (assign);

				this.EmitStoreSetCall (StoreMode.Local);

				++index;
			}

			// Compile program body
			exit = this.generator.DefineLabel ();

			this.CompileCommand (command, exit, 0);
			this.EmitLoadVoid ();

			this.generator.MarkLabel (exit);

			// Leave global scope and return
			this.EmitStoreLeave ();

			this.generator.Emit (OpCodes.Ret);

			return new Storage (this.constants);
		}

		#endregion

		#region Methods / Private

		private void CompileCommand (Command command, Label exit, int depth)
		{
			LocalBuilder buffer;
			LocalBuilder counter;
			Label empty;
			LocalBuilder enumerator;
			LocalBuilder fields;
			Label jump;
			LocalBuilder operand;
			LocalBuilder pair;
			Label skip;

			// Compile command
			switch (command.Type)
			{
				case CommandType.AssignFunction:
					this.EmitLoadStore ();
					this.EmitLoadValue (command.Name);

					this.EmitLoadValue (new FunctionValue (new Function (command.Arguments, command.Body, this.trimmer)));
					this.EmitStoreSetCall (command.Mode);

					break;

				case CommandType.AssignRender:
					// Prepare new buffer to store sub-rendering
					buffer = this.LocalReserve<TextWriter> ();

					this.generator.Emit (OpCodes.Newobj, Resolver.Constructor<Func<StringWriter>> (() => new StringWriter ()));
					this.generator.Emit (OpCodes.Stloc, buffer);

					// Load function, empty arguments array, store and text writer onto stack
					this.EmitLoadValue (new FunctionValue (new Function (Enumerable.Empty<string> (), command.Body, this.trimmer)));

					this.generator.Emit (OpCodes.Callvirt, Resolver.Property<Func<Value, IFunction>> ((v) => v.AsFunction).GetGetMethod ());
					this.generator.Emit (OpCodes.Ldc_I4, 0);
					this.generator.Emit (OpCodes.Newarr, typeof (Value));

					this.EmitLoadStore ();

					this.generator.Emit (OpCodes.Ldloc, buffer);

					this.EmitCallFunctionExecute ();

					this.generator.Emit (OpCodes.Pop);

					// Convert buffer into string and set to store
					this.EmitLoadStore ();
					this.EmitLoadValue (command.Name);

					this.generator.Emit (OpCodes.Ldloc, buffer);
					this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Func<StringWriter, string>> ((w) => w.ToString ()));

					this.LocalRelease<Value> (buffer);

					this.generator.Emit (OpCodes.Newobj, Resolver.Constructor<Func<string, Value>> ((s) => new StringValue (s)));

					this.EmitStoreSetCall (command.Mode);

					break;

				case CommandType.AssignValue:
					this.CompileExpression (command.Operand);

					operand = this.LocalReserve<Value> ();

					this.generator.Emit (OpCodes.Stloc, operand);

					this.EmitLoadStore ();
					this.EmitLoadValue (command.Name);

					this.generator.Emit (OpCodes.Ldloc, operand);

					this.LocalRelease<Value> (operand);
					this.EmitStoreSetCall (command.Mode);

					break;

				case CommandType.Composite:
					this.CompileCommand (command.Body, exit, depth);
					this.CompileCommand (command.Next, exit, depth);

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

					// Enter loop scope
					this.EmitStoreEnter ();

					// Set current element key if required
					if (!string.IsNullOrEmpty (command.Key))
					{
						this.EmitLoadStore ();
						this.EmitLoadValue (command.Key);

						this.generator.Emit (OpCodes.Ldloca, pair);
						this.generator.Emit (OpCodes.Call, Resolver.Property<Func<KeyValuePair<Value, Value>, Value>> ((p) => p.Key).GetGetMethod ());

						this.EmitStoreSetCall (StoreMode.Local);
					}

					// Set current element value
					this.EmitLoadStore ();
					this.EmitLoadValue (command.Name);

					this.generator.Emit (OpCodes.Ldloca, pair);
					this.generator.Emit (OpCodes.Call, Resolver.Property<Func<KeyValuePair<Value, Value>, Value>> ((p) => p.Value).GetGetMethod ());

					this.LocalRelease<KeyValuePair<Value, Value>> (pair);
					this.EmitStoreSetCall (StoreMode.Local);

					// Evaluate body and restart cycle
					this.CompileCommand (command.Body, exit, depth + 1);
					this.EmitStoreLeave ();

					this.generator.Emit (OpCodes.Br, jump);

					// Evaluate command for "empty" case
					this.generator.MarkLabel (empty);

					if (command.Next != null)
					{
						this.EmitStoreEnter ();
						this.CompileCommand (command.Next, exit, depth + 1);
						this.EmitStoreLeave ();
					}

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
						this.EmitStoreEnter ();
						this.CompileCommand (command.Body, exit, depth + 1);
						this.EmitStoreLeave ();

						this.generator.Emit (OpCodes.Br, skip);
						this.generator.MarkLabel (jump);
					}

					// Emit fallback branch if any
					if (command != null)
					{
						this.EmitStoreEnter ();
						this.CompileCommand (command, exit, depth + 1);
						this.EmitStoreLeave ();
					}

					// Mark end of statement
					this.generator.MarkLabel (skip);

					break;

				case CommandType.Literal:
					this.EmitLoadOutput ();

					this.generator.Emit (OpCodes.Ldstr, this.trimmer (command.Text));

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

						this.EmitStoreLeave ();

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

					this.EmitStoreEnter ();
					this.CompileCommand (command.Body, exit, depth + 1);
					this.EmitStoreLeave ();

					// Evaluate loop condition, restart cycle if true
					this.generator.MarkLabel (skip);

					this.CompileExpression (command.Operand);
					this.EmitCallValueAsBoolean ();

					this.generator.Emit (OpCodes.Brtrue, jump);

					break;
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
					this.EmitLoadStore ();
					this.EmitLoadOutput ();

					value = this.LocalReserve<Value> ();

					this.EmitCallFunctionExecute ();

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
					this.EmitLoadStore ();
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

		private void EmitLoadStore ()
		{
			this.generator.Emit (OpCodes.Ldarg_2);
		}

		private void EmitLoadValue (Value constant)
		{
			int index;

			if (!this.indices.TryGetValue (constant, out index))
			{
				index = this.constants.Count;

				this.indices[constant] = index;
				this.constants.Add (constant);
			}

			this.EmitLoadContext ();

			this.generator.Emit (OpCodes.Ldfld, Resolver.Field<Func<Storage, Value[]>> ((c) => c.Constants));
			this.generator.Emit (OpCodes.Ldc_I4, index);
			this.generator.Emit (OpCodes.Ldelem_Ref);
		}

		private void EmitLoadVoid ()
		{
			this.generator.Emit (OpCodes.Call, Resolver.Property<Func<Value>> (() => VoidValue.Instance).GetGetMethod ());
		}

		private void EmitStoreEnter ()
		{
			this.EmitLoadStore ();

			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IStore>> ((s) => s.Enter ()));
		}

		private void EmitStoreLeave ()
		{
			this.EmitLoadStore ();

			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IStore>> ((s) => s.Leave ()));
			this.generator.Emit (OpCodes.Pop);
		}

		private void EmitStoreSetCall (StoreMode mode)
		{
			this.generator.Emit (OpCodes.Ldc_I4, (int)mode);
			this.generator.Emit (OpCodes.Callvirt, Resolver.Method<Action<IStore, Value, Value, StoreMode>> ((s, n, v, m) => s.Set (n, v, m)));
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
