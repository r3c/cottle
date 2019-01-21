using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using Cottle.Values;

namespace Cottle.Documents.Dynamic
{
    internal class Compiler
    {
        #region Constructors

        public Compiler(ILGenerator generator, Trimmer trimmer)
        {
            _constants = new List<Value>();
            _generator = generator;
            _indices = new Dictionary<Value, int>();
            _locals = new Dictionary<Type, Queue<LocalBuilder>>();
            _trimmer = trimmer;
        }

        #endregion

        #region Attributes

        private readonly List<Value> _constants;

        private readonly ILGenerator _generator;

        private readonly Dictionary<Value, int> _indices;

        private readonly Dictionary<Type, Queue<LocalBuilder>> _locals;

        private readonly Trimmer _trimmer;

        #endregion

        #region Methods / Public

        public Storage Compile(IEnumerable<string> arguments, Command command)
        {
            // Create global scope for program execution
            EmitStoreEnter();

            // Assign provided values to arguments
            var index = 0;

            foreach (var argument in arguments)
            {
                var assign = _generator.DefineLabel();
                var copy = _generator.DefineLabel();

                EmitLoadStore();
                EmitLoadValue(argument);

                // Check if a value is available for current argument 
                EmitLoadArguments();

                _generator.Emit(OpCodes.Callvirt,
                    Resolver.Property<Func<IList<Value>, int>>(a => a.Count).GetGetMethod());
                _generator.Emit(OpCodes.Ldc_I4, index);
                _generator.Emit(OpCodes.Bgt_S, copy);

                // Push void value for current argument
                EmitLoadVoid();

                _generator.Emit(OpCodes.Br, assign);

                // Fetch argument value from arguments array
                _generator.MarkLabel(copy);

                EmitLoadArguments();

                _generator.Emit(OpCodes.Ldc_I4, index);
                _generator.Emit(OpCodes.Callvirt, Resolver.Method<Func<IList<Value>, int, Value>>((a, i) => a[i]));

                // Assign argument value
                _generator.MarkLabel(assign);

                EmitStoreSetCall(StoreMode.Local);

                ++index;
            }

            // Compile program body
            var exit = _generator.DefineLabel();

            CompileCommand(command, exit, 0);
            EmitLoadVoid();

            _generator.MarkLabel(exit);

            // Leave global scope and return
            EmitStoreLeave();

            _generator.Emit(OpCodes.Ret);

            return new Storage(_constants);
        }

        #endregion

        #region Methods / Private

        private void CompileCommand(Command command, Label exit, int depth)
        {
            Label jump;
            LocalBuilder operand;
            Label skip;

            // Compile command
            switch (command.Type)
            {
                case CommandType.AssignFunction:
                    EmitLoadStore();
                    EmitLoadValue(command.Name);

                    EmitLoadValue(new FunctionValue(new Function(command.Arguments, command.Body, _trimmer)));
                    EmitStoreSetCall(command.Mode);

                    break;

                case CommandType.AssignRender:
                    // Prepare new buffer to store sub-rendering
                    var buffer = LocalReserve<TextWriter>();

                    _generator.Emit(OpCodes.Newobj, Resolver.Constructor<Func<StringWriter>>(() => new StringWriter()));
                    _generator.Emit(OpCodes.Stloc, buffer);

                    // Load function, empty arguments array, store and text writer onto stack
                    EmitLoadValue(new FunctionValue(new Function(Enumerable.Empty<string>(), command.Body, _trimmer)));

                    _generator.Emit(OpCodes.Callvirt,
                        Resolver.Property<Func<Value, IFunction>>(v => v.AsFunction).GetGetMethod());
                    _generator.Emit(OpCodes.Ldc_I4, 0);
                    _generator.Emit(OpCodes.Newarr, typeof(Value));

                    EmitLoadStore();

                    _generator.Emit(OpCodes.Ldloc, buffer);

                    EmitCallFunctionExecute();

                    _generator.Emit(OpCodes.Pop);

                    // Convert buffer into string and set to store
                    EmitLoadStore();
                    EmitLoadValue(command.Name);

                    _generator.Emit(OpCodes.Ldloc, buffer);
                    _generator.Emit(OpCodes.Callvirt, Resolver.Method<Func<StringWriter, string>>(w => w.ToString()));

                    LocalRelease<Value>(buffer);

                    _generator.Emit(OpCodes.Newobj, Resolver.Constructor<Func<string, Value>>(s => new StringValue(s)));

                    EmitStoreSetCall(command.Mode);

                    break;

                case CommandType.AssignValue:
                    CompileExpression(command.Operand);

                    operand = LocalReserve<Value>();

                    _generator.Emit(OpCodes.Stloc, operand);

                    EmitLoadStore();
                    EmitLoadValue(command.Name);

                    _generator.Emit(OpCodes.Ldloc, operand);

                    LocalRelease<Value>(operand);
                    EmitStoreSetCall(command.Mode);

                    break;

                case CommandType.Composite:
                    CompileCommand(command.Body, exit, depth);
                    CompileCommand(command.Next, exit, depth);

                    break;

                case CommandType.Dump:
                    CompileExpression(command.Operand);

                    operand = LocalReserve<Value>();

                    _generator.Emit(OpCodes.Stloc, operand);

                    EmitLoadOutput();

                    _generator.Emit(OpCodes.Ldloc, operand);
                    _generator.Emit(OpCodes.Callvirt, Resolver.Method<Action<TextWriter, object>>((w, v) => w.Write(v)));

                    LocalRelease<Value>(operand);

                    break;

                case CommandType.Echo:
                    CompileExpression(command.Operand);

                    operand = LocalReserve<Value>();

                    _generator.Emit(OpCodes.Stloc, operand);

                    EmitLoadOutput();

                    _generator.Emit(OpCodes.Ldloc, operand);
                    _generator.Emit(OpCodes.Callvirt,
                        Resolver.Property<Func<Value, string>>(v => v.AsString).GetGetMethod());

                    LocalRelease<Value>(operand);

                    EmitCallWriteString();

                    break;

                case CommandType.For:
                    var empty = _generator.DefineLabel();
                    jump = _generator.DefineLabel();
                    skip = _generator.DefineLabel();

                    // Evaluate operand into fields map
                    CompileExpression(command.Operand);
                    EmitCallValueFields();

                    var fields = LocalReserve<IMap>();

                    _generator.Emit(OpCodes.Stloc, fields);

                    // Get number of fields
                    _generator.Emit(OpCodes.Ldloc, fields);
                    _generator.Emit(OpCodes.Callvirt, Resolver.Property<Func<IMap, int>>(m => m.Count).GetGetMethod());
                    _generator.Emit(OpCodes.Brfalse, empty);

                    // Evaluate command for "not empty" case
                    _generator.Emit(OpCodes.Ldloc, fields);
                    _generator.Emit(OpCodes.Callvirt,
                        Resolver.Method<Func<IMap, IEnumerator<KeyValuePair<Value, Value>>>>(m => m.GetEnumerator()));

                    LocalRelease<IMap>(fields);

                    var enumerator = LocalReserve<IEnumerator<KeyValuePair<Value, Value>>>();

                    _generator.Emit(OpCodes.Stloc, enumerator);

                    // Fetch next enumerator element or end loop
                    _generator.MarkLabel(jump);
                    _generator.Emit(OpCodes.Ldloc, enumerator);
                    _generator.Emit(OpCodes.Callvirt,
                        Resolver.Method<Func<IEnumerator<KeyValuePair<Value, Value>>, bool>>(e => e.MoveNext()));
                    _generator.Emit(OpCodes.Brfalse, skip);
                    _generator.Emit(OpCodes.Ldloc, enumerator);

                    LocalRelease<IEnumerator<KeyValuePair<Value, Value>>>(enumerator);

                    _generator.Emit(OpCodes.Callvirt,
                        Resolver
                            .Property<Func<IEnumerator<KeyValuePair<Value, Value>>, KeyValuePair<Value, Value>>>(e =>
                                e.Current).GetGetMethod());

                    var pair = LocalReserve<KeyValuePair<Value, Value>>();

                    _generator.Emit(OpCodes.Stloc, pair);

                    // Enter loop scope
                    EmitStoreEnter();

                    // Set current element key if required
                    if (!string.IsNullOrEmpty(command.Key))
                    {
                        EmitLoadStore();
                        EmitLoadValue(command.Key);

                        _generator.Emit(OpCodes.Ldloca, pair);
                        _generator.Emit(OpCodes.Call,
                            Resolver.Property<Func<KeyValuePair<Value, Value>, Value>>(p => p.Key).GetGetMethod());

                        EmitStoreSetCall(StoreMode.Local);
                    }

                    // Set current element value
                    EmitLoadStore();
                    EmitLoadValue(command.Name);

                    _generator.Emit(OpCodes.Ldloca, pair);
                    _generator.Emit(OpCodes.Call,
                        Resolver.Property<Func<KeyValuePair<Value, Value>, Value>>(p => p.Value).GetGetMethod());

                    LocalRelease<KeyValuePair<Value, Value>>(pair);
                    EmitStoreSetCall(StoreMode.Local);

                    // Evaluate body and restart cycle
                    CompileCommand(command.Body, exit, depth + 1);
                    EmitStoreLeave();

                    _generator.Emit(OpCodes.Br, jump);

                    // Evaluate command for "empty" case
                    _generator.MarkLabel(empty);

                    if (command.Next != null)
                    {
                        EmitStoreEnter();
                        CompileCommand(command.Next, exit, depth + 1);
                        EmitStoreLeave();
                    }

                    // Mark end of statement
                    _generator.MarkLabel(skip);

                    break;

                case CommandType.If:
                    skip = _generator.DefineLabel();

                    // Emit conditional branches
                    for (; command != null && command.Type == CommandType.If; command = command.Next)
                    {
                        jump = _generator.DefineLabel();

                        // Evaluate branch condition, jump to next if false
                        CompileExpression(command.Operand);
                        EmitCallValueAsBoolean();

                        _generator.Emit(OpCodes.Brfalse, jump);

                        // Execute branch command and jump sibling statements
                        EmitStoreEnter();
                        CompileCommand(command.Body, exit, depth + 1);
                        EmitStoreLeave();

                        _generator.Emit(OpCodes.Br, skip);
                        _generator.MarkLabel(jump);
                    }

                    // Emit fallback branch if any
                    if (command != null)
                    {
                        EmitStoreEnter();
                        CompileCommand(command, exit, depth + 1);
                        EmitStoreLeave();
                    }

                    // Mark end of statement
                    _generator.MarkLabel(skip);

                    break;

                case CommandType.Literal:
                    EmitLoadOutput();

                    _generator.Emit(OpCodes.Ldstr, _trimmer(command.Text));

                    EmitCallWriteString();

                    break;

                case CommandType.Return:
                    CompileExpression(command.Operand);

                    // Leave all opened scopes if any
                    if (depth > 0)
                    {
                        _generator.Emit(OpCodes.Ldc_I4, depth);

                        var counter = LocalReserve<int>();

                        _generator.Emit(OpCodes.Stloc, counter);

                        jump = _generator.DefineLabel();

                        _generator.MarkLabel(jump);

                        EmitStoreLeave();

                        _generator.Emit(OpCodes.Ldloc, counter);
                        _generator.Emit(OpCodes.Ldc_I4_1);
                        _generator.Emit(OpCodes.Sub);
                        _generator.Emit(OpCodes.Stloc, counter);
                        _generator.Emit(OpCodes.Ldloc, counter);
                        _generator.Emit(OpCodes.Brtrue, jump);

                        LocalRelease<int>(counter);
                    }

                    _generator.Emit(OpCodes.Br, exit);

                    break;

                case CommandType.While:
                    jump = _generator.DefineLabel();
                    skip = _generator.DefineLabel();

                    // Branch to condition before first body execution
                    _generator.Emit(OpCodes.Br, skip);

                    // Execute loop command
                    _generator.MarkLabel(jump);

                    EmitStoreEnter();
                    CompileCommand(command.Body, exit, depth + 1);
                    EmitStoreLeave();

                    // Evaluate loop condition, restart cycle if true
                    _generator.MarkLabel(skip);

                    CompileExpression(command.Operand);
                    EmitCallValueAsBoolean();

                    _generator.Emit(OpCodes.Brtrue, jump);

                    break;
            }
        }

        private void CompileExpression(Expression expression)
        {
            LocalBuilder arguments;
            Label success;
            LocalBuilder value;

            switch (expression.Type)
            {
                case ExpressionType.Access:
                    success = _generator.DefineLabel();

                    // Evaluate source expression and get fields
                    CompileExpression(expression.Source);
                    EmitCallValueFields();

                    var fields = LocalReserve<IMap>();

                    _generator.Emit(OpCodes.Stloc, fields);

                    // Evaluate subscript expression
                    CompileExpression(expression.Subscript);

                    value = LocalReserve<Value>();

                    _generator.Emit(OpCodes.Stloc, value);

                    // Use subscript to get value from fields
                    var tryGetFromValue = typeof(IMap).GetMethod("TryGet") ?? throw new InvalidOperationException();

                    _generator.Emit(OpCodes.Ldloc, fields);
                    _generator.Emit(OpCodes.Ldloc, value);
                    _generator.Emit(OpCodes.Ldloca, value);
                    _generator.Emit(OpCodes.Callvirt, tryGetFromValue);
                    _generator.Emit(OpCodes.Brtrue, success);

                    // Emit void value on error
                    EmitLoadVoid();

                    _generator.Emit(OpCodes.Stloc, value);

                    // Push value on stack
                    _generator.MarkLabel(success);
                    _generator.Emit(OpCodes.Ldloc, value);

                    LocalRelease<Value>(value);

                    break;

                case ExpressionType.Constant:
                    EmitLoadValue(expression.Value);

                    break;

                case ExpressionType.Invoke:
                    var failure = _generator.DefineLabel();
                    success = _generator.DefineLabel();

                    // Evaluate source expression as a function
                    CompileExpression(expression.Source);

                    _generator.Emit(OpCodes.Callvirt,
                        Resolver.Property<Func<Value, IFunction>>(v => v.AsFunction).GetGetMethod());

                    var function = LocalReserve<IFunction>();

                    _generator.Emit(OpCodes.Stloc, function);
                    _generator.Emit(OpCodes.Ldloc, function);
                    _generator.Emit(OpCodes.Brfalse, failure);

                    // Create array to store evaluated values
                    _generator.Emit(OpCodes.Ldc_I4, expression.Arguments.Length);
                    _generator.Emit(OpCodes.Newarr, typeof(Value));

                    arguments = LocalReserve<Value[]>();

                    _generator.Emit(OpCodes.Stloc, arguments);

                    // Evaluate arguments and store into array
                    for (var i = 0; i < expression.Arguments.Length; ++i)
                    {
                        CompileExpression(expression.Arguments[i]);

                        value = LocalReserve<Value>();

                        _generator.Emit(OpCodes.Stloc, value);
                        _generator.Emit(OpCodes.Ldloc, arguments);
                        _generator.Emit(OpCodes.Ldc_I4, i);
                        _generator.Emit(OpCodes.Ldloc, value);
                        _generator.Emit(OpCodes.Stelem_Ref);

                        LocalRelease<Value>(value);
                    }

                    // Invoke function delegate within exception block
                    _generator.Emit(OpCodes.Ldloc, function);
                    _generator.Emit(OpCodes.Ldloc, arguments);

                    LocalRelease<Value[]>(arguments);
                    LocalRelease<IFunction>(function);
                    EmitLoadStore();
                    EmitLoadOutput();

                    value = LocalReserve<Value>();

                    EmitCallFunctionExecute();

                    _generator.Emit(OpCodes.Stloc, value);
                    _generator.Emit(OpCodes.Br_S, success);

                    // Emit void value on error
                    _generator.MarkLabel(failure);

                    EmitLoadVoid();

                    _generator.Emit(OpCodes.Stloc, value);

                    // Value is already available on stack
                    _generator.MarkLabel(success);
                    _generator.Emit(OpCodes.Ldloc, value);

                    LocalRelease<Value>(value);

                    break;

                case ExpressionType.Map:
                    // Create array to store evaluated pairs
                    _generator.Emit(OpCodes.Ldc_I4, expression.Elements.Length);
                    _generator.Emit(OpCodes.Newarr, typeof(KeyValuePair<Value, Value>));

                    arguments = LocalReserve<KeyValuePair<Value, Value>[]>();

                    _generator.Emit(OpCodes.Stloc, arguments);

                    // Evaluate elements and store into array 
                    var constructor = Resolver.Constructor<Func<Value, Value, KeyValuePair<Value, Value>>>((k, v) =>
                        new KeyValuePair<Value, Value>(k, v));

                    for (var i = 0; i < expression.Elements.Length; ++i)
                    {
                        CompileExpression(expression.Elements[i].Key);

                        var key = LocalReserve<Value>();

                        _generator.Emit(OpCodes.Stloc, key);

                        CompileExpression(expression.Elements[i].Value);

                        value = LocalReserve<Value>();

                        _generator.Emit(OpCodes.Stloc, value);
                        _generator.Emit(OpCodes.Ldloc, arguments);
                        _generator.Emit(OpCodes.Ldc_I4, i);
                        _generator.Emit(OpCodes.Ldelema, typeof(KeyValuePair<Value, Value>));
                        _generator.Emit(OpCodes.Ldloc, key);
                        _generator.Emit(OpCodes.Ldloc, value);
                        _generator.Emit(OpCodes.Newobj, constructor);
                        _generator.Emit(OpCodes.Stobj, typeof(KeyValuePair<Value, Value>));

                        LocalRelease<Value>(key);
                        LocalRelease<Value>(value);
                    }

                    // Create value from array
                    constructor =
                        Resolver.Constructor<Func<IEnumerable<KeyValuePair<Value, Value>>, Value>>(f =>
                            new MapValue(f));

                    _generator.Emit(OpCodes.Ldloc, arguments);
                    _generator.Emit(OpCodes.Newobj, constructor);

                    LocalRelease<KeyValuePair<Value, Value>[]>(arguments);

                    break;

                case ExpressionType.Symbol:
                    success = _generator.DefineLabel();

                    // Get variable from scope
                    EmitLoadStore();
                    EmitLoadValue(expression.Value);

                    value = LocalReserve<Value>();

                    var tryGetFromStore = typeof(IStore).GetMethod("TryGet") ?? throw new InvalidOperationException();

                    _generator.Emit(OpCodes.Ldloca, value);
                    _generator.Emit(OpCodes.Callvirt, tryGetFromStore);
                    _generator.Emit(OpCodes.Brtrue, success);

                    // Emit void value on error
                    EmitLoadVoid();

                    _generator.Emit(OpCodes.Stloc, value);

                    // Push value on stack
                    _generator.MarkLabel(success);
                    _generator.Emit(OpCodes.Ldloc, value);

                    LocalRelease<Value>(value);

                    break;

                case ExpressionType.Void:
                    EmitLoadVoid();

                    break;
            }
        }

        private void EmitCallFunctionExecute()
        {
            _generator.Emit(OpCodes.Callvirt,
                Resolver.Method<Func<IFunction, IList<Value>, IStore, TextWriter, Value>>((f, a, s, o) =>
                    f.Execute(a, s, o)));
        }

        private void EmitCallValueAsBoolean()
        {
            _generator.Emit(OpCodes.Callvirt, Resolver.Property<Func<Value, bool>>(v => v.AsBoolean).GetGetMethod());
        }

        private void EmitCallValueFields()
        {
            _generator.Emit(OpCodes.Callvirt, Resolver.Property<Func<Value, IMap>>(v => v.Fields).GetGetMethod());
        }

        private void EmitCallWriteString()
        {
            _generator.Emit(OpCodes.Callvirt, Resolver.Method<Action<TextWriter, string>>((w, v) => w.Write(v)));
        }

        private void EmitLoadArguments()
        {
            _generator.Emit(OpCodes.Ldarg_1);
        }

        private void EmitLoadContext()
        {
            _generator.Emit(OpCodes.Ldarga, 0);
        }

        private void EmitLoadOutput()
        {
            _generator.Emit(OpCodes.Ldarg_3);
        }

        private void EmitLoadStore()
        {
            _generator.Emit(OpCodes.Ldarg_2);
        }

        private void EmitLoadValue(Value constant)
        {
            if (!_indices.TryGetValue(constant, out var index))
            {
                index = _constants.Count;

                _indices[constant] = index;
                _constants.Add(constant);
            }

            EmitLoadContext();

            _generator.Emit(OpCodes.Ldfld, Resolver.Field<Func<Storage, Value[]>>(c => c.Constants));
            _generator.Emit(OpCodes.Ldc_I4, index);
            _generator.Emit(OpCodes.Ldelem_Ref);
        }

        private void EmitLoadVoid()
        {
            _generator.Emit(OpCodes.Call, Resolver.Property<Func<Value>>(() => VoidValue.Instance).GetGetMethod());
        }

        private void EmitStoreEnter()
        {
            EmitLoadStore();

            _generator.Emit(OpCodes.Callvirt, Resolver.Method<Action<IStore>>(s => s.Enter()));
        }

        private void EmitStoreLeave()
        {
            EmitLoadStore();

            _generator.Emit(OpCodes.Callvirt, Resolver.Method<Action<IStore>>(s => s.Leave()));
            _generator.Emit(OpCodes.Pop);
        }

        private void EmitStoreSetCall(StoreMode mode)
        {
            _generator.Emit(OpCodes.Ldc_I4, (int) mode);
            _generator.Emit(OpCodes.Callvirt,
                Resolver.Method<Action<IStore, Value, Value, StoreMode>>((s, n, v, m) => s.Set(n, v, m)));
        }

        private void LocalRelease<T>(LocalBuilder local)
        {
            if (!_locals.TryGetValue(typeof(T), out var queue))
            {
                queue = new Queue<LocalBuilder>();

                _locals[typeof(T)] = queue;
            }

            queue.Enqueue(local);
        }

        private LocalBuilder LocalReserve<T>()
        {
            if (_locals.TryGetValue(typeof(T), out var queue) && queue.Count > 0)
                return queue.Dequeue();

            return _generator.DeclareLocal(typeof(T));
        }

        #endregion
    }
}