using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Functions;

namespace Cottle.Documents.Evaluated
{
    internal class Frame
    {
        public static Func<Frame, Value> CreateGetter(Symbol symbol)
        {
            var index = symbol.Index;

            switch (symbol.Mode)
            {
                case StoreMode.Global:
                    return frame => frame._globals[index];

                case StoreMode.Local:
                    return frame => frame._locals[index];

                default:
                    throw new InvalidOperationException();
            }
        }

        public static Action<Frame, Value> CreateSetter(Symbol symbol)
        {
            var index = symbol.Index;

            switch (symbol.Mode)
            {
                case StoreMode.Global:
                    return (frame, value) => frame._globals[index] = value;

                case StoreMode.Local:
                    return (frame, value) => frame._locals[index] = value;

                default:
                    throw new InvalidOperationException();
            }
        }

        private readonly Value[] _globals;
        private readonly Value[] _locals;
        private readonly Stack<IFunction> _modifiers;

        public Frame(Value[] globals, int localCount, Stack<IFunction> modifiers)
        {
            _globals = globals;
            _locals = localCount > 0 ? new Value[localCount] : Array.Empty<Value>();
            _modifiers = modifiers;
        }

        public Frame CreateForFunction(IReadOnlyList<Action<Frame, Value>> argumentSetters, IReadOnlyList<Value> values,
            int localCount)
        {
            var functionArguments = Math.Min(argumentSetters.Count, values.Count);
            var functionFrame = new Frame(_globals, localCount, _modifiers);

            for (var i = 0; i < functionArguments; ++i)
                argumentSetters[i](functionFrame, values[i]);

            for (var i = values.Count; i < argumentSetters.Count; ++i)
                argumentSetters[i](functionFrame, Value.Undefined);

            return functionFrame;
        }

        public string Echo(Value value, TextWriter output)
        {
            foreach (var modifier in _modifiers)
            {
                if (modifier is FiniteFunction finiteModifier)
                    value = finiteModifier.Invoke1(this, value, output);
                else
                    value = modifier.Invoke(this, new[] { value }, output);
            }

            return value.AsString;
        }

        public IFunction Unwrap()
        {
            return _modifiers.Count > 0 ? _modifiers.Pop() : Function.Empty;
        }

        public void Wrap(IFunction modifier)
        {
            _modifiers.Push(modifier);
        }
    }
}