using System.Collections.Generic;
using System.IO;
using Cottle.Exceptions;
using Cottle.Functions;

namespace Cottle
{
    internal class Runtime
    {
        public readonly Value[] Globals;

        private readonly Stack<IFunction> _modifiers;
        private readonly int? _nbCycleMax;

        private int _nbCycle;

        public Runtime(Value[] globals, int? nbCycleMax)
        {
            Globals = globals;

            _modifiers = new Stack<IFunction>();
            _nbCycleMax = nbCycleMax;
            _nbCycle = 0;
        }

        public string Echo(Value value, TextWriter output)
        {
            if (_modifiers.Count < 1)
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

        public void Tick()
        {
            if (_nbCycleMax.HasValue && ++_nbCycle > _nbCycleMax.Value)
                throw new NbCycleExceededException(_nbCycleMax.Value);
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