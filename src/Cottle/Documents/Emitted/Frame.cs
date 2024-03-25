using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Functions;

namespace Cottle.Documents.Emitted
{
    internal class Frame
    {
        public static Frame CreateRoot()
        {
            return new Frame(Array.Empty<Value>(), Array.Empty<Value>(), null);
        }

        public readonly IReadOnlyList<Value> Arguments;
        public readonly IReadOnlyList<Value> Constants;

        private Stack<IFunction>? _modifiers;

        private Frame(IReadOnlyList<Value> constants, IReadOnlyList<Value> arguments, Stack<IFunction>? modifiers)
        {
            Arguments = arguments;
            Constants = constants;

            _modifiers = modifiers;
        }

        public Frame CreateForFunction(IReadOnlyList<Value> constants, IReadOnlyList<Value> arguments)
        {
            return new Frame(constants, arguments, _modifiers);
        }

        public string Echo(Tuple<Runtime, Frame> state, Value value, TextWriter output)
        {
            if (_modifiers is null)
                return value.AsString;

            foreach (var modifier in _modifiers)
            {
                if (modifier is FiniteFunction finiteModifier)
                    value = finiteModifier.Invoke1(state, value, output);
                else
                    value = modifier.Invoke(state, new[] { value }, output);
            }

            return value.AsString;
        }

        public IFunction Unwrap()
        {
            if (_modifiers is not null && _modifiers.Count > 0)
                return _modifiers.Pop();

            return Function.Empty;
        }

        public void Wrap(IFunction modifier)
        {
            if (_modifiers is null)
                _modifiers = new Stack<IFunction>();

            _modifiers.Push(modifier);
        }
    }
}