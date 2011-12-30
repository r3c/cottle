using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cottle.Values;

namespace   Cottle.Commons
{
    public static class CommonTools
    {
        #region Methods / Public

        public static bool  ValuesLoad (BinaryReader reader, IDictionary<string, Value> values)
        {
            int     count;
            string  key;
            Value   value;

            for (count = reader.ReadInt32 (); count-- > 0; )
            {
                key = reader.ReadString ();

                if (!CommonTools.ValueLoad (reader, out value))
                    return false;

                values[key] = value;
            }

            return true;
        }

        public static void  ValuesSave (BinaryWriter writer, IDictionary<string, Value> values)
        {
            writer.Write (values.Count);

            foreach (KeyValuePair<string, Value> pair in values)
            {
                writer.Write (pair.Key);

                CommonTools.ValueSave (writer, pair.Value);
            }
        }

        #endregion

        #region Methods / Private

        private static bool ValueLoad (BinaryReader reader, out Value value)
        {
            List<KeyValuePair<Value, Value>>    array;
            Value                               arrayKey;
            Value                               arrayValue;
            int                                 count;
            Value.DataType                      type = (Value.DataType)reader.ReadInt32 ();

            switch (type)
            {
                case Value.DataType.ARRAY:
                    count = reader.ReadInt32 ();
                    array = new List<KeyValuePair<Value, Value>> (count);

                    while (count-- > 0)
                    {
                        if (!CommonTools.ValueLoad (reader, out arrayKey) || !CommonTools.ValueLoad (reader, out arrayValue))
                        {
                            value = null;

                            return false;
                        }

                        array.Add (new KeyValuePair<Value, Value> (arrayKey, arrayValue));
                    }

                    value = array;

                    break;

                case Value.DataType.BOOLEAN:
                    value = reader.ReadBoolean () ? BooleanValue.True : BooleanValue.False;

                    break;

                case Value.DataType.FUNCTION:
                    value = UndefinedValue.Instance;

                    break;

                case Value.DataType.NUMBER:
                    value = reader.ReadDecimal ();

                    break;

                case Value.DataType.STRING:
                    value = reader.ReadString ();

                    break;

                case Value.DataType.UNDEFINED:
                    value = UndefinedValue.Instance;

                    break;

                default:
                    value = null;

                    return false;
            }

            return true;
        }

        private static void ValueSave (BinaryWriter writer, Value value)
        {
            writer.Write ((int)value.Type);

            switch (value.Type)
            {
                case Value.DataType.ARRAY:
                    writer.Write (value.Fields.Count);

                    foreach (KeyValuePair<Value, Value> pair in value.Fields)
                    {
                        CommonTools.ValueSave (writer, pair.Key);
                        CommonTools.ValueSave (writer, pair.Value);
                    }

                    break;

                case Value.DataType.BOOLEAN:
                    writer.Write (value.AsBoolean);

                    break;

                case Value.DataType.NUMBER:
                    writer.Write (value.AsNumber);

                    break;

                case Value.DataType.STRING:
                    writer.Write (value.AsString);

                    break;
            }
        }

        #endregion
    }
}
