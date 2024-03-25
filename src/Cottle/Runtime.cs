using System.Collections.Generic;
using System.IO;
using Cottle.Functions;

namespace Cottle
{
    internal class Runtime
    {
        public readonly Value[] Globals;

        private readonly Stack<IFunction> _modifiers;

        public Runtime(Value[] globals)
        {
            Globals = globals;
            _modifiers = new Stack<IFunction>();
        }

        public string Echo(object state, Value value, TextWriter output)
        {
            if (_modifiers.Count < 1)
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
            return _modifiers.Count > 0 ? _modifiers.Pop() : Function.Empty;
        }

        public void Wrap(IFunction modifier)
        {
            _modifiers.Push(modifier);
        }
    }
}