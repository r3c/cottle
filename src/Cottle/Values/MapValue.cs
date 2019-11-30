using System;
using System.Collections.Generic;
using System.Text;
using Cottle.Maps;

namespace Cottle.Values
{
    public sealed class MapValue : Value
    {
        public static MapValue Empty { get; } = new MapValue();

        public override bool AsBoolean => Fields.Count > 0;

        public override IFunction AsFunction => null;

        public override decimal AsNumber => Fields.Count;

        public override string AsString => string.Empty;

        public override IMap Fields { get; }

        public override ValueContent Type => ValueContent.Map;

        public MapValue(Func<int, Value> generator, int count)
        {
            Fields = new GeneratorMap(generator, count);
        }

        public MapValue(IDictionary<Value, Value> hash)
        {
            Fields = new HashMap(hash);
        }

        public MapValue(IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            Fields = new MixMap(pairs);
        }

        public MapValue(IEnumerable<Value> values)
        {
            Fields = new ArrayMap(values);
        }

        public MapValue()
        {
            Fields = EmptyMap.Instance;
        }

        public override int CompareTo(Value other)
        {
            if (other == null)
                return 1;

            if (Type != other.Type)
                return ((int)Type).CompareTo((int)other.Type);

            return Fields.CompareTo(other.Fields);
        }

        public override int GetHashCode()
        {
            return Fields.GetHashCode();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var separator = false;

            builder.Append('[');

            foreach (var pair in Fields)
            {
                if (separator)
                    builder.Append(", ");
                else
                    separator = true;

                builder.Append(pair.Key);
                builder.Append(": ");
                builder.Append(pair.Value);
            }

            builder.Append(']');

            return builder.ToString();
        }
    }
}