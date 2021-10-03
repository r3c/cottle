using System.Collections.Generic;
using Cottle.Maps;

namespace Cottle.SpyRecords
{
    internal sealed class EvaluableSpyRecord : IEvaluable, ISpyRecord
    {
        public bool AsBoolean => _current.AsBoolean;

        public IFunction AsFunction => _current.AsFunction;

        public double AsNumber => _current.AsNumber;

        public string AsString => _current.AsString;

        public IMap Fields => _map;

        public ValueContent Type => _current.Type;

        public Value Value => _current;

        private Value _current;

        private readonly SpyMap _map;

        public EvaluableSpyRecord(Value current)
        {
            _current = current;
            _map = new SpyMap(current.Fields);
        }

        public int CompareTo(Value other)
        {
            return _current.CompareTo(other);
        }

        public bool Equals(Value other)
        {
            return _current.Equals(other);
        }

        public ISpyRecord SpyField(Value key)
        {
            return _map.SpyField(key);
        }

        public IReadOnlyDictionary<Value, ISpyRecord> SpyFields()
        {
            return _map.SpyFields();
        }

        public void Update(Value value)
        {
            _current = value;
            _map.Update(value.Fields);
        }
    }
}