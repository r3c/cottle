using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Exceptions;
using Cottle.Functions;
using Cottle.Maps;

namespace Cottle
{
    internal class Runtime : IRuntime
    {
        public IMap Globals => new MixMap(
            _globalKeys.Zip(GlobalValues, (key, value) => new KeyValuePair<Value, Value>(key, value)));

        public readonly Value[] GlobalValues;

        private readonly IReadOnlyList<Value> _globalKeys;
        private readonly Stack<IFunction> _modifiers;
        private readonly int? _nbCycleMax;

        private int _nbCycle;

        public Runtime(IReadOnlyList<Value> globalKeys, Value[] globalValues, int? nbCycleMax)
        {
            GlobalValues = globalValues;

            _globalKeys = globalKeys;
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
                value = modifier is FiniteFunction finiteModifier
                    ? finiteModifier.Invoke1(this, value, output)
                    : modifier.Invoke(this, new[] { value }, output);
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