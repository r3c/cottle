using Cottle.Contexts.Monitor.SymbolUsages;

namespace Cottle.Contexts.Monitor
{
    internal class MonitorValue : Value
    {
        private readonly Value _value;

        public MonitorValue(Value value, MutableSymbolUsage usage)
        {
            Fields = new MonitorMap(value.Fields, usage);
            _value = value;
        }

        public override bool AsBoolean => _value.AsBoolean;
        public override IFunction AsFunction => _value.AsFunction;
        public override decimal AsNumber => _value.AsNumber;
        public override string AsString => _value.AsString;
        public override IMap Fields { get; }

        public override ValueContent Type => _value.Type;

        public override int CompareTo(Value other)
        {
            return _value.CompareTo(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}