using System;
using System.Collections.Generic;
using System.IO;

namespace Cottle.Demo.Serialization
{
    public static class ValueSerializer
    {
        public static bool TryRead(BinaryReader reader, int version, out IReadOnlyDictionary<string, Value> values)
        {
            var dictionary = new Dictionary<string, Value>();

            for (var count = reader.ReadInt32(); count-- > 0;)
            {
                var key = reader.ReadString();

                if (!ValueSerializer.TryReadValue(reader, version, out var value))
                {
                    values = default;

                    return false;
                }

                dictionary[key] = value;
            }

            values = dictionary;

            return true;
        }

        public static bool TryWrite(BinaryWriter writer, IReadOnlyCollection<KeyValuePair<string, Value>> values)
        {
            writer.Write(values.Count);

            foreach (var pair in values)
            {
                writer.Write(pair.Key);

                if (!ValueSerializer.TryWriteValue(writer, pair.Value))
                    return false;
            }

            return true;
        }

        private static bool TryReadType(BinaryReader reader, int version, out ValueContent type)
        {
            var value = reader.ReadInt32();

            if (version >= 3)
            {
                type = (ValueContent)value;

                return value >= 0 && value < Enum.GetNames(typeof(ValueContent)).Length;
            }

            switch (value)
            {
                case 0:
                    type = ValueContent.Map;

                    return true;

                case 1:
                    type = ValueContent.Boolean;

                    return true;

                case 2:
                    type = ValueContent.Function;

                    return true;

                case 3:
                    type = ValueContent.Number;

                    return true;

                case 4:
                    type = ValueContent.String;

                    return true;

                case 5:
                    type = ValueContent.Void;

                    return true;

                default:
                    type = default;

                    return false;
            }
        }

        private static bool TryReadValue(BinaryReader reader, int version, out Value value)
        {
            if (!ValueSerializer.TryReadType(reader, version, out var type))
            {
                value = default;

                return false;
            }

            switch (type)
            {
                case ValueContent.Boolean:
                    value = reader.ReadBoolean() ? Value.True : Value.False;

                    break;

                case ValueContent.Map:
                    var count = reader.ReadInt32();
                    var list = new List<KeyValuePair<Value, Value>>(count);

                    while (count-- > 0)
                    {
                        if (!ValueSerializer.TryReadValue(reader, version, out var mapKey) ||
                            !ValueSerializer.TryReadValue(reader, version, out var mapValue))
                        {
                            value = default;

                            return false;
                        }

                        list.Add(new KeyValuePair<Value, Value>(mapKey, mapValue));
                    }

                    value = list;

                    break;

                case ValueContent.Number:
                    value = version < 3 ? (double)reader.ReadDecimal() : reader.ReadDouble();

                    break;

                case ValueContent.String:
                    value = reader.ReadString();

                    break;

                case ValueContent.Function:
                case ValueContent.Void:
                    value = Value.Undefined;

                    break;

                default:
                    throw new InvalidOperationException();
            }

            return true;
        }

        private static bool TryWriteValue(BinaryWriter writer, Value value)
        {
            writer.Write((int)value.Type);

            switch (value.Type)
            {
                case ValueContent.Boolean:
                    writer.Write(value.AsBoolean);

                    return true;

                case ValueContent.Function:
                case ValueContent.Void:
                    return true;

                case ValueContent.Map:
                    writer.Write(value.Fields.Count);

                    foreach (var pair in value.Fields)
                    {
                        ValueSerializer.TryWriteValue(writer, pair.Key);
                        ValueSerializer.TryWriteValue(writer, pair.Value);
                    }

                    return true;

                case ValueContent.Number:
                    writer.Write(value.AsNumber);

                    return true;

                case ValueContent.String:
                    writer.Write(value.AsString);

                    return true;

                default:
                    return false;
            }
        }
    }
}