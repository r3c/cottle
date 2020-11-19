using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Cottle.Functions;

namespace Cottle.Documents.Emitted
{
    internal class Emitter
    {
        private static readonly MethodInfo ArgumentsIndex =
            Resolver.Method<Func<IReadOnlyList<Value>, Value>>(c => c[default]);

        private static readonly MethodInfo FiniteFunctionInvoke0 =
            Resolver.Method<Func<FiniteFunction, Value>>(f => f.Invoke0(default, default));

        private static readonly MethodInfo FiniteFunctionInvoke1 =
            Resolver.Method<Func<FiniteFunction, Value>>(f => f.Invoke1(default, default, default));

        private static readonly MethodInfo FiniteFunctionInvoke2 =
            Resolver.Method<Func<FiniteFunction, Value>>(f => f.Invoke2(default, default, default, default));

        private static readonly MethodInfo FiniteFunctionInvoke3 =
            Resolver.Method<Func<FiniteFunction, Value>>(f => f.Invoke3(default, default, default, default, default));

        private static readonly FieldInfo FrameArguments =
            Resolver.Field<Func<Frame, IReadOnlyList<Value>>>(f => f.Arguments);

        private static readonly MethodInfo FrameEcho =
            Resolver.Method<Func<Frame, string>>(f => f.Echo(default, default));

        private static readonly FieldInfo FrameGlobals = Resolver.Field<Func<Frame, Value[]>>(f => f.Globals);

        private static readonly MethodInfo FrameUnwrap = Resolver.Method<Func<Frame, IFunction>>(f => f.Unwrap());

        private static readonly MethodInfo FrameWrap = Resolver.Method<Action<Frame>>(f => f.Wrap(default));

        private static readonly MethodInfo FunctionInvoke =
            Resolver.Method<Func<IFunction, Value>>(f => f.Invoke(default, default, default));

        private static readonly ConstructorInfo KeyValueConstructor =
            Resolver.Constructor<Func<KeyValuePair<Value, Value>>>(() =>
                new KeyValuePair<Value, Value>(default, default));

        private static readonly IReadOnlyList<OpCode> LoadIntegers = new[]
        {
            OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_1, OpCodes.Ldc_I4_2, OpCodes.Ldc_I4_3, OpCodes.Ldc_I4_4, OpCodes.Ldc_I4_5,
            OpCodes.Ldc_I4_6, OpCodes.Ldc_I4_7, OpCodes.Ldc_I4_8
        };

        private static readonly MethodInfo MapCount =
            Resolver.Property<Func<IMap, int>>(m => m.Count).GetGetMethod();

        private static readonly MethodInfo MapEnumeratorCurrent = Resolver
            .Property<Func<IEnumerator<KeyValuePair<Value, Value>>, KeyValuePair<Value, Value>>>(e => e.Current)
            .GetGetMethod();

        private static readonly MethodInfo MapEnumeratorMoveNext =
            Resolver.Method<Func<IEnumerator<KeyValuePair<Value, Value>>, bool>>(e => e.MoveNext());

        private static readonly MethodInfo MapGetEnumerator =
            Resolver.Method<Func<IMap, IEnumerator<KeyValuePair<Value, Value>>>>(m => m.GetEnumerator());

        private static readonly MethodInfo MapTryGet =
            Resolver.Method<Func<IMap, bool>>(m => m.TryGet(default, out Emitter._value));

        private static readonly MethodInfo ObjectToString = Resolver.Method<Func<object, string>>(o => o.ToString());

        private static readonly MethodInfo PairKey =
            Resolver.Property<Func<KeyValuePair<Value, Value>, Value>>(p => p.Key).GetGetMethod();

        private static readonly MethodInfo PairValue =
            Resolver.Property<Func<KeyValuePair<Value, Value>, Value>>(p => p.Value).GetGetMethod();

        private static readonly PropertyInfo ReadOnlyListCount =
            Resolver.Property<Func<IReadOnlyList<object>, int>>(l => l.Count);

        private static readonly ConstructorInfo StringWriterConstructor =
            Resolver.Constructor<Func<StringWriter>>(() => new StringWriter());

        private static readonly MethodInfo StringWriterToString =
            Resolver.Method<Func<StringWriter, string>>(w => w.ToString());

        private static readonly MethodInfo TextWriterWriteObject =
            Resolver.Method<Action<TextWriter>>(w => w.Write(default(object)));

        private static readonly MethodInfo TextWriterWriteString =
            Resolver.Method<Action<TextWriter>>(w => w.Write(default(string)));

        private static readonly MethodInfo ValueAsBooleanGet =
            Resolver.Property<Func<Value, bool>>(v => v.AsBoolean).GetGetMethod();

        private static readonly MethodInfo ValueAsFunctionGet =
            Resolver.Property<Func<Value, IFunction>>(v => v.AsFunction).GetGetMethod();

        private static readonly MethodInfo ValueFieldsGet =
            Resolver.Property<Func<Value, IMap>>(v => v.Fields).GetGetMethod();

        private static readonly MethodInfo ValueFromDictionary =
            Resolver.Method<Func<IEnumerable<KeyValuePair<Value, Value>>, Value>>(f => Value.FromEnumerable(f));

        private static readonly MethodInfo ValueFromString =
            Resolver.Method<Func<Value>>(() => Value.FromString(default));

        private static readonly FieldInfo ValueUndefined = Resolver.Field<Func<Value>>(() => Value.Undefined);

        private static Value _value;

        private readonly Dictionary<Value, int> _constants;
        private readonly ILGenerator _generator;
        private readonly Dictionary<Type, Stack<LocalBuilder>> _locals;
        private readonly Queue<LocalBuilder> _outputs;
        private readonly Dictionary<int, LocalBuilder> _symbols;

        public Emitter(ILGenerator generator)
        {
            _constants = new Dictionary<Value, int>();
            _generator = generator;
            _locals = new Dictionary<Type, Stack<LocalBuilder>>();
            _outputs = new Queue<LocalBuilder>();
            _symbols = new Dictionary<int, LocalBuilder>();
        }

        public IReadOnlyList<Value> CreateConstants()
        {
            var constants = new Value[_constants.Count];

            foreach (var pair in _constants)
                constants[pair.Value] = pair.Key;

            return constants;
        }

        public Label DeclareLabel()
        {
            return _generator.DefineLabel();
        }

        public void EmitBranchAlways(Label label)
        {
            _generator.Emit(OpCodes.Br, label);
        }

        public void EmitBranchWhenFalse(Label label)
        {
            _generator.Emit(OpCodes.Brfalse, label);
        }

        public void EmitBranchWhenGreaterOrEqual(Label label)
        {
            _generator.Emit(OpCodes.Bge, label);
        }

        public void EmitBranchWhenTrue(Label label)
        {
            _generator.Emit(OpCodes.Brtrue, label);
        }

        public void EmitCastAs<TValue>()
        {
            _generator.Emit(OpCodes.Isinst, typeof(TValue));
        }

        public Local<TValue> EmitDeclareLocalAndLoadAddress<TValue>()
        {
            return EmitDeclareLocal<TValue>(OpCodes.Ldloca);
        }

        public Local<TValue> EmitDeclareLocalAndStore<TValue>()
        {
            return EmitDeclareLocal<TValue>(OpCodes.Stloc);
        }

        public void EmitDiscard()
        {
            _generator.Emit(OpCodes.Pop);
        }

        public void EmitCallFiniteFunctionInvoke0()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.FiniteFunctionInvoke0);
        }

        public void EmitCallFiniteFunctionInvoke1()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.FiniteFunctionInvoke1);
        }

        public void EmitCallFiniteFunctionInvoke2()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.FiniteFunctionInvoke2);
        }

        public void EmitCallFiniteFunctionInvoke3()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.FiniteFunctionInvoke3);
        }

        public void EmitCallFrameEcho()
        {
            _generator.Emit(OpCodes.Call, Emitter.FrameEcho);
        }

        public void EmitCallFrameUnwrap()
        {
            _generator.Emit(OpCodes.Call, Emitter.FrameUnwrap);
        }

        public void EmitCallFrameWrap()
        {
            _generator.Emit(OpCodes.Call, Emitter.FrameWrap);
        }

        public void EmitCallFunctionInvoke()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.FunctionInvoke);
        }

        public void EmitCallMapCount()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapCount);
        }

        public void EmitCallMapEnumeratorCurrent()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapEnumeratorCurrent);
        }

        public void EmitCallMapEnumeratorMoveNext()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapEnumeratorMoveNext);
        }

        public void EmitCallMapGetEnumerator()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapGetEnumerator);
        }

        public void EmitCallMapTryGet()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapTryGet);
        }

        public void EmitCallObjectToString()
        {
            _generator.Emit(OpCodes.Constrained, typeof(Value));
            _generator.Emit(OpCodes.Callvirt, Emitter.ObjectToString);
        }

        public void EmitCallPairKey()
        {
            _generator.Emit(OpCodes.Call, Emitter.PairKey);
        }

        public void EmitCallPairValue()
        {
            _generator.Emit(OpCodes.Call, Emitter.PairValue);
        }

        public void EmitCallStringWriterToString()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.StringWriterToString);
        }

        public void EmitCallTextWriterWriteObject()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.TextWriterWriteObject);
        }

        public void EmitCallTextWriterWriteString()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.TextWriterWriteString);
        }

        public void EmitCallValueAsBoolean()
        {
            _generator.Emit(OpCodes.Call, Emitter.ValueAsBooleanGet);
        }

        public void EmitCallValueAsFunction()
        {
            _generator.Emit(OpCodes.Call, Emitter.ValueAsFunctionGet);
        }

        public void EmitCallValueFields()
        {
            _generator.Emit(OpCodes.Call, Emitter.ValueFieldsGet);
        }

        public void EmitCallValueFromDictionary()
        {
            _generator.Emit(OpCodes.Call, Emitter.ValueFromDictionary);
        }

        public void EmitCallValueFromString()
        {
            _generator.Emit(OpCodes.Call, Emitter.ValueFromString);
        }

        public void EmitLoadArray<TElement>(int count)
        {
            EmitLoadInteger(count);

            _generator.Emit(OpCodes.Newarr, typeof(TElement));
        }

        public void EmitLoadBoolean(bool value)
        {
            EmitLoadInteger(value ? 1 : 0);
        }

        public void EmitLoadConstant(Value constant)
        {
            if (!_constants.TryGetValue(constant, out var index))
            {
                index = _constants.Count;

                _constants[constant] = index;
            }

            _generator.Emit(OpCodes.Ldarg_0);

            EmitLoadInteger(index);

            _generator.Emit(OpCodes.Callvirt, Emitter.ArgumentsIndex);
        }

        public void EmitLoadDuplicate()
        {
            _generator.Emit(OpCodes.Dup);
        }

        public void EmitLoadElementAddressAtIndex<TElement>()
        {
            _generator.Emit(OpCodes.Ldelema, typeof(TElement));
        }

        public void EmitLoadElementValueAtIndex<TElement>()
        {
            _generator.Emit(OpCodes.Ldelem, typeof(TElement));
        }

        public void EmitLoadFrame()
        {
            _generator.Emit(OpCodes.Ldarg_1);
        }

        public void EmitLoadFrameArgument(int index)
        {
            _generator.Emit(OpCodes.Ldarg_1);
            _generator.Emit(OpCodes.Ldfld, Emitter.FrameArguments);

            EmitLoadInteger(index);

            _generator.Emit(OpCodes.Ldelem, typeof(Value));
        }

        public void EmitLoadFrameArgumentLength()
        {
            _generator.Emit(OpCodes.Ldarg_1);
            _generator.Emit(OpCodes.Ldfld, Emitter.FrameArguments);
            _generator.Emit(OpCodes.Callvirt, Emitter.ReadOnlyListCount.GetMethod);
        }

        public void EmitLoadFrameGlobal()
        {
            _generator.Emit(OpCodes.Ldarg_1);
            _generator.Emit(OpCodes.Ldfld, Emitter.FrameGlobals);
        }

        public void EmitLoadInteger(int value)
        {
            if (value < Emitter.LoadIntegers.Count)
                _generator.Emit(Emitter.LoadIntegers[value]);
            else if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                _generator.Emit(OpCodes.Ldc_I4_S, value);
            else
                _generator.Emit(OpCodes.Ldc_I4, value);
        }

        public void EmitLoadLocalAddress<TValue>(Local<TValue> local) where TValue : struct
        {
            _generator.Emit(OpCodes.Ldloca, local.Builder);
        }

        public void EmitLoadLocalAddressAndRelease<TValue>(Local<TValue> local) where TValue : struct
        {
            EmitLoadLocalAndRelease(OpCodes.Ldloca, local);
        }

        public void EmitLoadLocalValue<TValue>(Local<TValue> local)
        {
            _generator.Emit(OpCodes.Ldloc, local.Builder);
        }

        public void EmitLoadLocalValueAndRelease<TValue>(Local<TValue> local)
        {
            EmitLoadLocalAndRelease(OpCodes.Ldloc, local);
        }

        public void EmitLoadOutput()
        {
            if (_outputs.Count > 0)
                _generator.Emit(OpCodes.Ldloc, _outputs.Peek());
            else
                _generator.Emit(OpCodes.Ldarg_2);
        }

        public void EmitLoadResult()
        {
            _generator.Emit(OpCodes.Ldarg_3);
        }

        public void EmitLoadString(string value)
        {
            _generator.Emit(OpCodes.Ldstr, value);
        }

        public void EmitLoadUndefined()
        {
            _generator.Emit(OpCodes.Ldsfld, Emitter.ValueUndefined);
        }

        public void EmitNewKeyValuePair()
        {
            _generator.Emit(OpCodes.Newobj, Emitter.KeyValueConstructor);
        }

        public void EmitNewStringWriter()
        {
            _generator.Emit(OpCodes.Newobj, Emitter.StringWriterConstructor);
        }

        public void EmitReturn()
        {
            _generator.Emit(OpCodes.Ret);
        }

        public void EmitStoreElementAtIndex<TElement>()
        {
            _generator.Emit(OpCodes.Stelem, typeof(TElement));
        }

        public void EmitStoreLocal<TValue>(Local<TValue> local)
        {
            _generator.Emit(OpCodes.Stloc, local.Builder);
        }

        public void EmitStoreValueAtAddress<TValue>() where TValue : struct
        {
            _generator.Emit(OpCodes.Stobj, typeof(TValue));
        }

        public Local<Value> GetOrDeclareSymbol(int index)
        {
            if (_symbols.TryGetValue(index, out var symbol))
                return new Local<Value>(symbol);

            symbol = _generator.DeclareLocal(typeof(Value));

            _symbols[index] = symbol;

            return new Local<Value>(symbol);
        }

        public void MarkLabel(Label label)
        {
            _generator.MarkLabel(label);
        }

        public void OutputDequeue()
        {
            _outputs.Dequeue();
        }

        public void OutputEnqueue(Local<StringWriter> output)
        {
            _outputs.Enqueue(output.Builder);
        }

        private Local<TValue> EmitDeclareLocal<TValue>(OpCode opCode)
        {
            var local = _locals.TryGetValue(typeof(TValue), out var queue) && queue.Count > 0
                ? queue.Pop()
                : _generator.DeclareLocal(typeof(TValue));

            _generator.Emit(opCode, local);

            return new Local<TValue>(local);
        }

        private void EmitLoadLocalAndRelease<TValue>(OpCode opCode, Local<TValue> local)
        {
            _generator.Emit(opCode, local.Builder);

            if (!_locals.TryGetValue(typeof(TValue), out var stack))
            {
                stack = new Stack<LocalBuilder>();

                _locals[typeof(TValue)] = stack;
            }

            stack.Push(local.Builder);
        }
    }
}