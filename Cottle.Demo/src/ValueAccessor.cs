using System;
using System.Collections.Generic;
using System.IO;

using Cottle.Values;

namespace Cottle.Demo
{
	public static class ValueAccessor
	{
		#region Methods / Public

		public static bool	Load (BinaryReader reader, IDictionary<string, Value> values)
		{
			int		count;
			string	key;
			Value	value;

			for (count = reader.ReadInt32 (); count-- > 0; )
			{
				key = reader.ReadString ();

				if (!ValueAccessor.Load (reader, out value))
					return false;

				values[key] = value;
			}

			return true;
		}

		public static void	Save (BinaryWriter writer, IDictionary<string, Value> values)
		{
			writer.Write (values.Count);

			foreach (KeyValuePair<string, Value> pair in values)
			{
				writer.Write (pair.Key);

				ValueAccessor.Save (writer, pair.Value);
			}
		}

		#endregion

		#region Methods / Private

		private static bool Load (BinaryReader reader, out Value value)
		{
			List<KeyValuePair<Value, Value>>	array;
			Value								arrayKey;
			Value								arrayValue;
			int									count;
			ValueContent						type;

			type = (ValueContent)reader.ReadInt32 ();

			switch (type)
			{
				case ValueContent.Boolean:
					value = reader.ReadBoolean () ? BooleanValue.True : BooleanValue.False;

					break;

				case ValueContent.Map:
					count = reader.ReadInt32 ();
					array = new List<KeyValuePair<Value, Value>> (count);

					while (count-- > 0)
					{
						if (!ValueAccessor.Load (reader, out arrayKey) || !ValueAccessor.Load (reader, out arrayValue))
						{
							value = null;

							return false;
						}

						array.Add (new KeyValuePair<Value, Value> (arrayKey, arrayValue));
					}

					value = array;

					break;

				case ValueContent.Number:
					value = reader.ReadDecimal ();

					break;

				case ValueContent.String:
					value = reader.ReadString ();

					break;

				case ValueContent.Void:
					value = VoidValue.Instance;

					break;

				default:
					value = null;

					return false;
			}

			return true;
		}

		private static void Save (BinaryWriter writer, Value value)
		{
			writer.Write ((int)value.Type);

			switch (value.Type)
			{
				case ValueContent.Boolean:
					writer.Write (value.AsBoolean);

					break;

				case ValueContent.Map:
					writer.Write (value.Fields.Count);

					foreach (KeyValuePair<Value, Value> pair in value.Fields)
					{
						ValueAccessor.Save (writer, pair.Key);
						ValueAccessor.Save (writer, pair.Value);
					}

					break;

				case ValueContent.Number:
					writer.Write (value.AsNumber);

					break;

				case ValueContent.String:
					writer.Write (value.AsString);

					break;
			}
		}

		#endregion
	}
}
