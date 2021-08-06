using System.Collections.Generic;
using System.IO;
using Cottle.Functions;

namespace Cottle.Documents.Emitted
{
    internal class Frame
    {
        public readonly IReadOnlyList<Value> Arguments;
        public readonly Value[] Globals;

        private Stack<IFunction>? _modifiers;

        public Frame(Value[] globals, IReadOnlyList<Value> arguments, Stack<IFunction>? modifiers)
        {
            Arguments = arguments;
            Globals = globals;

            _modifiers = modifiers;
        }

        public Frame CreateForFunction(IReadOnlyList<Value> arguments)
        {
            return new Frame(Globals, arguments, _modifiers);
        }

        public string Echo(Value value, TextWriter output)
        {
            if (_modifiers == null)
                return value.AsString;

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