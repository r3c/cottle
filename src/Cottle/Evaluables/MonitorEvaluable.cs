using Cottle.Contexts.Monitor;
using Cottle.Contexts.Monitor.SymbolUsages;

namespace Cottle.Evaluables
{
    internal class MonitorEvaluable : IEvaluable
    {
        private readonly Value _value;

        public MonitorEvaluable(Value value, MutableSymbolUsage usage)
        {
            Fields = new MonitorMap(value.Fields, usage);
            _value = value;
        }

        public bool AsBoolean => _value.AsBoolean;
        public IFunction AsFunction => _value.AsFunction;
        public double AsNumber => _value.AsNumber;
        public string AsString => _value.AsString;
        public IMap Fields { get; }

        public ValueContent Type => _value.Type;

        public int CompareTo(Value other)
        {
            return _value.CompareTo(other);
        }

        public override bool Equals(object? obj)
        {
            return _value.Equals(obj);
        }

        public bool Equals(Value other)
        {
            return _value.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_value.GetHashCode() * 397) ^ (Fields is not null ? Fields.GetHashCode() : 0);
            }
        }

        public override string? ToString()
        {
            return _value.ToString();
        }
    }
}