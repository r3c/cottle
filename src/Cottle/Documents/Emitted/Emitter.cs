using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Cottle.Documents.Compiled;
using Cottle.Functions;

namespace Cottle.Documents.Emitted
{
    internal class Emitter
    {
        private static readonly MethodInfo FiniteFunctionInvoke0 =
            Dynamic.GetMethod<Func<FiniteFunction, Value>>(f => f.Invoke0(new(), TextWriter.Null));

        private static readonly MethodInfo FiniteFunctionInvoke1 =
            Dynamic.GetMethod<Func<FiniteFunction, Value>>(f => f.Invoke1(new(), default, TextWriter.Null));

        private static readonly MethodInfo FiniteFunctionInvoke2 =
            Dynamic.GetMethod<Func<FiniteFunction, Value>>(f => f.Invoke2(new(), default, default, TextWriter.Null));

        private static readonly MethodInfo FiniteFunctionInvoke3 =
            Dynamic.GetMethod<Func<FiniteFunction, Value>>(f => f.Invoke3(new(), default, default, default, TextWriter.Null));

        private static readonly FieldInfo FrameArguments =
            Dynamic.GetField<Func<Frame, IReadOnlyList<Value>>>(f => f.Arguments);

        private static readonly MethodInfo FrameEcho =
            Dynamic.GetMethod<Func<Frame, string>>(f => f.Echo(default, TextWriter.Null));

        private static readonly FieldInfo FrameGlobals = Dynamic.GetField<Func<Frame, Value[]>>(f => f.Globals);

        private static readonly MethodInfo FrameUnwrap = Dynamic.GetMethod<Func<Frame, IFunction>>(f => f.Unwrap());

        private static readonly MethodInfo FrameWrap = Dynamic.GetMethod<Action<Frame>>(f => f.Wrap(Function.Empty));

        private static readonly MethodInfo FunctionInvoke =
            Dynamic.GetMethod<Func<IFunction, Value>>(f => f.Invoke(new(), Array.Empty<Value>(), TextWriter.Null));

        private static readonly ConstructorInfo KeyValueConstructor =
            Dynamic.GetConstructor<Func<KeyValuePair<Value, Value>>>(() =>
                new KeyValuePair<Value, Value>(default, default));

        private static readonly IReadOnlyList<OpCode> LoadIntegers = new[]
        {
            OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_1, OpCodes.Ldc_I4_2, OpCodes.Ldc_I4_3, OpCodes.Ldc_I4_4, OpCodes.Ldc_I4_5,
            OpCodes.Ldc_I4_6, OpCodes.Ldc_I4_7, OpCodes.Ldc_I4_8
        };

        private static readonly MethodInfo MapCount =
            Dynamic.GetProperty<Func<IMap, int>>(m => m.Count).GetMethod!;

        private static readonly MethodInfo MapEnumeratorCurrent = Dynamic
            .GetProperty<Func<IEnumerator<KeyValuePair<Value, Value>>, KeyValuePair<Value, Value>>>(e => e.Current)
            .GetMethod!;

        private static readonly MethodInfo MapEnumeratorMoveNext =
            Dynamic.GetMethod<Func<IEnumerator<KeyValuePair<Value, Value>>, bool>>(e => e.MoveNext());

        private static readonly MethodInfo MapGetEnumerator =
            Dynamic.GetMethod<Func<IMap, IEnumerator<KeyValuePair<Value, Value>>>>(m => m.GetEnumerator());

        private static readonly MethodInfo MapTryGet =
            Dynamic.GetMethod<Func<IMap, bool>>(m => m.TryGet(default, out Emitter._value));

        private static readonly MethodInfo ObjectToString = Dynamic.GetMethod<Func<object, string?>>(o => o.ToString());

        private static readonly MethodInfo PairKey =
            Dynamic.GetProperty<Func<KeyValuePair<Value, Value>, Value>>(p => p.Key).GetMethod!;

        private static readonly MethodInfo PairValue =
            Dynamic.GetProperty<Func<KeyValuePair<Value, Value>, Value>>(p => p.Value).GetMethod!;

        private static readonly MethodInfo ReadOnlyListCount =
            Dynamic.GetProperty<Func<IReadOnlyList<object>, int>>(l => l.Count).GetMethod!;

        private static readonly ConstructorInfo StringWriterConstructor =
            Dynamic.GetConstructor<Func<StringWriter>>(() => new StringWriter());

        private static readonly MethodInfo StringWriterToString =
            Dynamic.GetMethod<Func<StringWriter, string>>(w => w.ToString());

        private static readonly MethodInfo TextWriterWriteObject =
            Dynamic.GetMethod<Action<TextWriter>>(w => w.Write(default(object)));

        private static readonly MethodInfo TextWriterWriteString =
            Dynamic.GetMethod<Action<TextWriter>>(w => w.Write(default(string)));

        private static readonly MethodInfo ValueAsBooleanGet =
            Dynamic.GetProperty<Func<Value, bool>>(v => v.AsBoolean).GetMethod!;

        private static readonly MethodInfo ValueAsFunctionGet =
            Dynamic.GetProperty<Func<Value, IFunction>>(v => v.AsFunction).GetMethod!;

        private static readonly MethodInfo ValueFieldsGet =
            Dynamic.GetProperty<Func<Value, IMap>>(v => v.Fields).GetMethod!;

        private static readonly MethodInfo ValueFromDictionary =
            Dynamic.GetMethod<Func<IEnumerable<KeyValuePair<Value, Value>>, Value>>(f => Value.FromEnumerable(f));

        private static readonly MethodInfo ValueFromString =
            Dynamic.GetMethod<Func<Value>>(() => Value.FromString(string.Empty));

        private static readonly FieldInfo ValueUndefined = Dynamic.GetField<Func<Value>>(() => Value.Undefined);

        private static Value _value;

        private readonly Dictionary<Value, int> _constants;
        private readonly ILGenerator _generator;
        private readonly Dictionary<Type, Stack<LocalBuilder>> _internalLocals;
        private readonly Queue<LocalBuilder> _outputs;
        private readonly Dictionary<int, LocalBuilder> _symbolLocals;

        public Emitter(ILGenerator generator)
        {
            _constants = new Dictionary<Value, int>();
            _generator = generator;
            _internalLocals = new Dictionary<Type, Stack<LocalBuilder>>();
            _outputs = new Queue<LocalBuilder>();
            _symbolLocals = new Dictionary<int, LocalBuilder>();
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
            EmitLoadFrame();

            _generator.Emit(OpCodes.Call, Emitter.FrameUnwrap);
        }

        public void EmitCallFrameWrap<TValue>(Local<TValue> modifier)
        {
            EmitLoadFrame();
            EmitLoadLocalValueAndRelease(modifier);

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

            EmitLoadElementValueAtIndex<Value>(index);
        }

        public void EmitLoadDuplicate()
        {
            _generator.Emit(OpCodes.Dup);
        }

        public void EmitLoadElementAddressAtIndex<TElement>(int index)
        {
            EmitLoadInteger(index);

            _generator.Emit(OpCodes.Ldelema, typeof(TElement));
        }

        public void EmitLoadElementValueAtIndex<TElement>(int index)
        {
            EmitLoadInteger(index);

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

            EmitLoadElementValueAtIndex<Value>(index);
        }

        public void EmitLoadFrameArgumentLength()
        {
            _generator.Emit(OpCodes.Ldarg_1);
            _generator.Emit(OpCodes.Ldfld, Emitter.FrameArguments);
            _generator.Emit(OpCodes.Callvirt, Emitter.ReadOnlyListCount);
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

        public void EmitLoadState()
        {
            EmitLoadFrame();
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

        public Local<Value> GetOrDeclareLocal(Symbol symbol)
        {
            if (_symbolLocals.TryGetValue(symbol.Index, out var local))
                return new Local<Value>(local);

            local = _generator.DeclareLocal(typeof(Value));

            _symbolLocals[symbol.Index] = local;

            return new Local<Value>(local);
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
            var local = _internalLocals.TryGetValue(typeof(TValue), out var queue) && queue.Count > 0
                ? queue.Pop()
                : _generator.DeclareLocal(typeof(TValue));

            _generator.Emit(opCode, local);

            return new Local<TValue>(local);
        }

        private void EmitLoadLocalAndRelease<TValue>(OpCode opCode, Local<TValue> local)
        {
            _generator.Emit(opCode, local.Builder);

            if (!_internalLocals.TryGetValue(typeof(TValue), out var stack))
            {
                stack = new Stack<LocalBuilder>();

                _internalLocals[typeof(TValue)] = stack;
            }

            stack.Push(local.Builder);
        }
    }
}