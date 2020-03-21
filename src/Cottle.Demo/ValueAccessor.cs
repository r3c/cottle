using System;
using System.Collections.Generic;
using System.IO;

namespace Cottle.Demo
{
    public static class ValueAccessor
    {
        public static bool TryLoad(BinaryReader reader, int version, IDictionary<string, Value> values)
        {
            int count;

            for (count = reader.ReadInt32(); count-- > 0;)
            {
                var key = reader.ReadString();

                if (!ValueAccessor.TryReadValue(reader, version, out var value))
                    return false;

                values[key] = value;
            }

            return true;
        }

        public static void Save(BinaryWriter writer, IDictionary<string, Value> values)
        {
            writer.Write(values.Count);

            foreach (var pair in values)
            {
                writer.Write(pair.Key);

                ValueAccessor.WriteValue(writer, pair.Value);
            }
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
            if (!ValueAccessor.TryReadType(reader, version, out var type))
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
                        if (!ValueAccessor.TryReadValue(reader, version, out var mapKey) ||
                            !ValueAccessor.TryReadValue(reader, version, out var mapValue))
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

        private static void WriteValue(BinaryWriter writer, Value value)
        {
            writer.Write((int)value.Type);

            switch (value.Type)
            {
                case ValueContent.Boolean:
                    writer.Write(value.AsBoolean);

                    break;

                case ValueContent.Function:
                case ValueContent.Void:
                    break;

                case ValueContent.Map:
                    writer.Write(value.Fields.Count);

                    foreach (var pair in value.Fields)
                    {
                        ValueAccessor.WriteValue(writer, pair.Key);
                        ValueAccessor.WriteValue(writer, pair.Value);
                    }

                    break;

                case ValueContent.Number:
                    writer.Write(value.AsNumber);

                    break;

                case ValueContent.String:
                    writer.Write(value.AsString);

                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}