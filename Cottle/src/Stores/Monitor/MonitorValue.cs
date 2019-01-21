using Cottle.Stores.Monitor.SymbolUsages;

namespace Cottle.Stores.Monitor
{
    class MonitorValue : Value
    {
        public override bool AsBoolean => this.value.AsBoolean;
        public override IFunction AsFunction => this.value.AsFunction;
        public override decimal AsNumber => this.value.AsNumber;
        public override string AsString => this.value.AsString;
        public override IMap Fields => this.fields;
        public override ValueContent Type => this.value.Type;

        private readonly IMap fields;
        private readonly Value value;

        public MonitorValue(Value value, MutableSymbolUsage usage)
        {
            this.fields = new MonitorMap(value.Fields, usage);
            this.value = value;
        }

        public override int CompareTo(Value other)
        {
            return this.value.CompareTo(other);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }
}
