using System;
using System.Collections.Generic;
using System.IO;

namespace Cottle.Demo
{
    public static class ValueAccessor
    {
        public static bool Load(BinaryReader reader, IDictionary<string, Value> values)
        {
            int count;

            for (count = reader.ReadInt32(); count-- > 0;)
            {
                var key = reader.ReadString();

                if (!ValueAccessor.Load(reader, out var value))
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

                ValueAccessor.Save(writer, pair.Value);
            }
        }

        private static bool Load(BinaryReader reader, out Value value)
        {
            var type = (ValueContent)reader.ReadInt32();

            switch (type)
            {
                case ValueContent.Boolean:
                    value = reader.ReadBoolean() ? Value.True : Value.False;

                    break;

                case ValueContent.Map:
                    var count = reader.ReadInt32();
                    var array = new List<KeyValuePair<Value, Value>>(count);

                    while (count-- > 0)
                    {
                        if (!ValueAccessor.Load(reader, out var arrayKey) ||
                            !ValueAccessor.Load(reader, out var arrayValue))
                        {
                            value = default;

                            return false;
                        }

                        array.Add(new KeyValuePair<Value, Value>(arrayKey, arrayValue));
                    }

                    value = array;

                    break;

                case ValueContent.Number:
                    value = reader.ReadDecimal();

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

        private static void Save(BinaryWriter writer, Value value)
        {
            writer.Write((int)value.Type);

            switch (value.Type)
            {
                case ValueContent.Boolean:
                    writer.Write(value.AsBoolean);

                    break;

                case ValueContent.Map:
                    writer.Write(value.Fields.Count);

                    foreach (var pair in value.Fields)
                    {
                        ValueAccessor.Save(writer, pair.Key);
                        ValueAccessor.Save(writer, pair.Value);
                    }

                    break;

                case ValueContent.Number:
                    writer.Write(value.AsNumber);

                    break;

                case ValueContent.String:
                    writer.Write(value.AsString);

                    break;
            }
        }
    }
}