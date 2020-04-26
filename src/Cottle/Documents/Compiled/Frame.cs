using System;
using System.Collections.Generic;
using System.IO;

namespace Cottle.Documents.Compiled
{
    internal class Frame
    {
        public static Func<Frame, Value> CreateGetter(Symbol symbol)
        {
            var index = symbol.Index;

            switch (symbol.Mode)
            {
                case StoreMode.Global:
                    return frame => frame.Globals[index];

                case StoreMode.Local:
                    return frame => frame.Locals[index];

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
                    return (frame, value) => frame.Globals[index] = value;

                case StoreMode.Local:
                    return (frame, value) => frame.Locals[index] = value;

                default:
                    throw new InvalidOperationException();
            }
        }

        public readonly Value[] Globals;
        public readonly Value[] Locals;

        private Stack<IFunction> _modifiers;

        public Frame(Value[] globals, int localCount, Stack<IFunction> modifiers)
        {
            Globals = globals;
            Locals = localCount > 0 ? new Value[localCount] : null;

            _modifiers = modifiers;
        }

        public Frame CreateForFunction(IReadOnlyList<Symbol> arguments, IReadOnlyList<Value> values, int localCount)
        {
            var functionArguments = Math.Min(arguments.Count, values.Count);
            var functionFrame = new Frame(Globals, localCount, _modifiers);

            // Note: we assume all function arguments are local symbols here to avoid re-testing their mode
            for (var i = 0; i < functionArguments; ++i)
                functionFrame.Locals[arguments[i].Index] = values[i];

            for (var i = values.Count; i < arguments.Count; ++i)
                functionFrame.Locals[arguments[i].Index] = Value.Undefined;

            return functionFrame;
        }

        public string Echo(Value value, TextWriter output)
        {
            if (_modifiers != null)
            {
                foreach (var modifier in _modifiers)
                    value = modifier.Invoke(this, new[] { value }, output);
            }

            return value.AsString;
        }

        public IFunction Unwrap()
        {
            if (_modifiers != null && _modifiers.Count > 0)
                return _modifiers.Pop();

            return Function.Empty;
        }

        public void Wrap(IFunction modifier)
        {
            if (_modifiers == null)
                _modifiers = new Stack<IFunction>();

            _modifiers.Push(modifier);
        }
    }
}