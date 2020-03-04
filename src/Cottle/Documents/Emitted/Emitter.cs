using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Cottle.Documents.Compiled;
using Cottle.Values;

namespace Cottle.Documents.Emitted
{
    internal class Emitter
    {
        private static readonly MethodInfo ArgumentsIndex =
            Resolver.Method<Func<IReadOnlyList<Value>, int, Value>>((c, i) => c[i]);

        private static readonly FieldInfo FrameGlobals = Resolver.Field<Func<Frame, Value[]>>(f => f.Globals);

        private static readonly FieldInfo FrameLocals = Resolver.Field<Func<Frame, Value[]>>(f => f.Locals);

        private static readonly MethodInfo FunctionInvoke =
            Resolver.Method<Func<IFunction, object, IReadOnlyList<Value>, TextWriter, Value>>((f, s, a, o) =>
                f.Invoke(s, a, o));

        private static readonly ConstructorInfo KeyValueConstructor =
            Resolver.Constructor<Func<Value, Value, KeyValuePair<Value, Value>>>((k, v) =>
                new KeyValuePair<Value, Value>(k, v));

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
            typeof(IMap).GetMethod(nameof(IMap.TryGet)) ?? throw new InvalidOperationException();

        private static readonly ConstructorInfo MapValueConstructor =
            Resolver.Constructor<Func<IEnumerable<KeyValuePair<Value, Value>>, Value>>(f =>
                new MapValue(f));

        private static readonly MethodInfo PairKey =
            Resolver.Property<Func<KeyValuePair<Value, Value>, Value>>(p => p.Key).GetGetMethod();

        private static readonly MethodInfo PairValue =
            Resolver.Property<Func<KeyValuePair<Value, Value>, Value>>(p => p.Value).GetGetMethod();

        private static readonly ConstructorInfo StringValueConstructor =
            Resolver.Constructor<Func<string, Value>>(s => new StringValue(s));

        private static readonly ConstructorInfo StringWriterConstructor =
            Resolver.Constructor<Func<StringWriter>>(() => new StringWriter());

        private static readonly MethodInfo StringWriterToString =
            Resolver.Method<Func<StringWriter, string>>(w => w.ToString());

        private static readonly MethodInfo TextWriterWriteObject =
            Resolver.Method<Action<TextWriter, object>>((w, v) => w.Write(v));

        private static readonly MethodInfo TextWriterWriteString =
            Resolver.Method<Action<TextWriter, string>>((w, v) => w.Write(v));

        private static readonly MethodInfo ValueAsBooleanGet =
            Resolver.Property<Func<Value, bool>>(v => v.AsBoolean).GetGetMethod();        

        private static readonly MethodInfo ValueAsFunctionGet =
            Resolver.Property<Func<Value, IFunction>>(v => v.AsFunction).GetGetMethod();

        private static readonly MethodInfo ValueAsStringGet =
            Resolver.Property<Func<Value, string>>(v => v.AsString).GetGetMethod();

        private static readonly MethodInfo ValueFieldsGet =
            Resolver.Property<Func<Value, IMap>>(v => v.Fields).GetGetMethod();

        private static readonly MethodInfo VoidValueInstance =
            Resolver.Property<Func<Value>>(() => VoidValue.Instance).GetGetMethod();
        
        private readonly Dictionary<Value, int> _constants;
        private readonly ILGenerator _generator;
        private readonly Dictionary<Type, Stack<LocalBuilder>> _locals;
        private readonly Queue<LocalBuilder> _outputs;

        public Emitter(ILGenerator generator)
        {
            _constants = new Dictionary<Value, int>();
            _generator = generator;
            _locals = new Dictionary<Type, Stack<LocalBuilder>>();
            _outputs = new Queue<LocalBuilder>();
        }

        public void BranchAlways(Label label)
        {
            _generator.Emit(OpCodes.Br, label);
        }

        public void BranchIfFalse(Label label)
        {
            _generator.Emit(OpCodes.Brfalse, label);
        }

        public void BranchIfTrue(Label label)
        {
            _generator.Emit(OpCodes.Brtrue, label);
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

        public Local<TValue> DeclareLocalAndLoadAddress<TValue>()
        {
            return DeclareLocalAndEmit<TValue>(OpCodes.Ldloca);
        }

        public Local<TValue> DeclareLocalAndStore<TValue>()
        {
            return DeclareLocalAndEmit<TValue>(OpCodes.Stloc);
        }

        public void Discard()
        {
            _generator.Emit(OpCodes.Pop);
        }

        public void InvokeFunction()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.FunctionInvoke);
        }

        public void InvokeMapCount()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapCount);
        }

        public void InvokeMapEnumeratorCurrent()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapEnumeratorCurrent);
        }

        public void InvokeMapEnumeratorMoveNext()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapEnumeratorMoveNext);
        }

        public void InvokeMapGetEnumerator()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapGetEnumerator);
        }

        public void InvokeMapTryGet()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.MapTryGet);
        }

        public void InvokePairKey()
        {
            _generator.Emit(OpCodes.Call, Emitter.PairKey);
        }

        public void InvokePairValue()
        {
            _generator.Emit(OpCodes.Call, Emitter.PairValue);
        }

        public void InvokeStringWriterToString()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.StringWriterToString);
        }

        public void InvokeTextWriterWriteObject()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.TextWriterWriteObject);
        }

        public void InvokeTextWriterWriteString()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.TextWriterWriteString);
        }

        public void InvokeValueAsBoolean()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.ValueAsBooleanGet);
        }

        public void InvokeValueAsFunction()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.ValueAsFunctionGet);
        }

        public void InvokeValueAsString()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.ValueAsStringGet);
        }

        public void InvokeValueFields()
        {
            _generator.Emit(OpCodes.Callvirt, Emitter.ValueFieldsGet);
        }

        public void LoadArray<TElement>(int count)
        {
            _generator.Emit(OpCodes.Ldc_I4, count);
            _generator.Emit(OpCodes.Newarr, typeof(TElement));
        }

        public void LoadBoolean(bool value)
        {
            _generator.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
        }

        public void LoadConstant(Value constant)
        {
            if (!_constants.TryGetValue(constant, out var index))
            {
                index = _constants.Count;

                _constants[constant] = index;
            }

            _generator.Emit(OpCodes.Ldarg_0);
            _generator.Emit(OpCodes.Ldc_I4, index);
            _generator.Emit(OpCodes.Callvirt, Emitter.ArgumentsIndex);
        }

        public void LoadElementAddress<TElement>()
        {
            _generator.Emit(OpCodes.Ldelema, typeof(TElement));
        }

        public void LoadFrame()
        {
            _generator.Emit(OpCodes.Ldarg_1);
            _generator.Emit(OpCodes.Box, typeof(Frame));
        }

        public void LoadFrameSymbol(Symbol symbol)
        {
            FieldInfo fieldInfo;

            switch (symbol.Mode)
            {
                case StoreMode.Global:
                    fieldInfo = Emitter.FrameGlobals;

                    break;

                case StoreMode.Local:
                    fieldInfo = Emitter.FrameLocals;

                    break;

                default:
                    throw new InvalidOperationException();
            }

            _generator.Emit(OpCodes.Ldarg_1);
            _generator.Emit(OpCodes.Ldfld, fieldInfo);
            _generator.Emit(OpCodes.Ldc_I4, symbol.Index);
        }

        public void LoadInteger(int value)
        {
            _generator.Emit(OpCodes.Ldc_I4, value);
        }

        public void LoadLocalReference<TValue>(Local<TValue> local)
        {
            _generator.Emit(OpCodes.Ldloc, local.Builder);
        }

        public void LoadLocalReferenceAndRelease<TValue>(Local<TValue> local)
        {
            LoadLocalAndRelease(OpCodes.Ldloc, local);
        }

        public void LoadLocalValue<TValue>(Local<TValue> local)
        {
            _generator.Emit(OpCodes.Ldloca, local.Builder);
        }

        public void LoadLocalValueAndRelease<TValue>(Local<TValue> local)
        {
            LoadLocalAndRelease(OpCodes.Ldloca, local);
        }

        public void LoadOutput()
        {
            if (_outputs.Count > 0)
                _generator.Emit(OpCodes.Ldloc, _outputs.Peek());
            else
                _generator.Emit(OpCodes.Ldarg_2);
        }

        public void LoadResultAddress()
        {
            _generator.Emit(OpCodes.Ldarg_3);
        }

        public void LoadString(string value)
        {
            _generator.Emit(OpCodes.Ldstr, value);
        }

        public void LoadSymbol(Symbol symbol)
        {
            _generator.Emit(OpCodes.Ldarg_1);

            switch (symbol.Mode)
            {
                case StoreMode.Global:
                    _generator.Emit(OpCodes.Ldfld, Emitter.FrameGlobals);

                    break;

                case StoreMode.Local:
                    _generator.Emit(OpCodes.Ldfld, Emitter.FrameLocals);

                    break;

                default:
                    throw new InvalidOperationException();
            }

            _generator.Emit(OpCodes.Ldc_I4, symbol.Index);
            _generator.Emit(OpCodes.Ldelem_Ref);
        }

        public void LoadVoid()
        {
            _generator.Emit(OpCodes.Call, Emitter.VoidValueInstance);
        }

        public void MarkLabel(Label label)
        {
            _generator.MarkLabel(label);
        }

        public void NewKeyValuePair()
        {
            _generator.Emit(OpCodes.Newobj, Emitter.KeyValueConstructor);
        }

        public void NewMapValue()
        {
            _generator.Emit(OpCodes.Newobj, Emitter.MapValueConstructor);
        }

        public void NewStringValue()
        {
            _generator.Emit(OpCodes.Newobj, Emitter.StringValueConstructor);
        }

        public void NewStringWriter()
        {
            _generator.Emit(OpCodes.Newobj, Emitter.StringWriterConstructor);
        }

        public void OutputDequeue()
        {
            _outputs.Dequeue();
        }

        public void OutputEnqueue(Local<StringWriter> output)
        {
            _outputs.Enqueue(output.Builder);
        }

        public void Return()
        {
            _generator.Emit(OpCodes.Ret);
        }

        public void StoreLocal<TValue>(Local<TValue> local)
        {
            _generator.Emit(OpCodes.Stloc, local.Builder);
        }

        public void StoreReferenceAtAddress()
        {
            _generator.Emit(OpCodes.Stind_Ref);
        }

        public void StoreReferenceAtIndex()
        {
            _generator.Emit(OpCodes.Stelem_Ref);
        }

        public void StoreValueAtAddress<TValue>()
        {
            _generator.Emit(OpCodes.Stobj, typeof(TValue));
        }

        private Local<TValue> DeclareLocalAndEmit<TValue>(OpCode opCode)
        {
            var local = _locals.TryGetValue(typeof(TValue), out var queue) && queue.Count > 0
                ? queue.Pop()
                : _generator.DeclareLocal(typeof(TValue));

            _generator.Emit(opCode, local);

            return new Local<TValue>(local);
        }

        private void LoadLocalAndRelease<TValue>(OpCode opCode, Local<TValue> local)
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