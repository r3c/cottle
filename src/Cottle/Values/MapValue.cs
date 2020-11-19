using System;
using System.Collections.Generic;
using System.Text;
using Cottle.Maps;

namespace Cottle.Values
{
    public sealed class MapValue : BaseValue
    {
        [Obsolete("Use `Value.EmptyMap`")] public static MapValue Empty { get; } = new MapValue();

        public override bool AsBoolean => Fields.Count > 0;

        public override IFunction AsFunction => Function.Empty;

        public override double AsNumber => Fields.Count;

        public override string AsString => string.Empty;

        public override IMap Fields { get; }

        public override ValueContent Type => ValueContent.Map;

        [Obsolete("Use `Value.FromGenerator()`")]
        public MapValue(Func<int, Value> generator, int count)
        {
            Fields = new GeneratorMap(generator, count);
        }

        [Obsolete("Use `Value.FromDictionary()`")]
        public MapValue(IDictionary<Value, Value> hash)
        {
            Fields = new HashMap(hash);
        }

        [Obsolete("Use `Value.FromEnumerable()`")]
        public MapValue(IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            Fields = new MixMap(pairs);
        }

        [Obsolete("Use `Value.FromEnumerable`")]
        public MapValue(IEnumerable<Value> values)
        {
            Fields = new ArrayMap(values);
        }

        [Obsolete("Use `Value.EmptyMap`")]
        public MapValue()
        {
            Fields = EmptyMap.Instance;
        }

        public override int CompareTo(Value other)
        {
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
            var comma = false;
            var index = 0;

            builder.Append('[');

            foreach (var pair in Fields)
            {
                if (comma)
                    builder.Append(", ");
                else
                    comma = true;

                if (pair.Key.Type == ValueContent.Number && Math.Abs(pair.Key.AsNumber - index) < double.Epsilon)
                    ++index;
                else
                {
                    builder.Append(pair.Key);
                    builder.Append(": ");
                }

                builder.Append(pair.Value);
            }

            builder.Append(']');

            return builder.ToString();
        }
    }
}