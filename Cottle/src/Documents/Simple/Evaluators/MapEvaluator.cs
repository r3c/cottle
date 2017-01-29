using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cottle.Values;

namespace Cottle.Documents.Simple.Evaluators
{
	class MapEvaluator : IEvaluator
	{
		#region Attributes

		private KeyValuePair<IEvaluator, IEvaluator>[] elements;

		#endregion

		#region Constructors

		public MapEvaluator (IEnumerable<KeyValuePair<IEvaluator, IEvaluator>> elements)
		{
			this.elements = elements.ToArray ();
		}

		#endregion

		#region Methods

		public Value Evaluate (IStore store, TextWriter writer)
		{
			return new MapValue (this.elements.Select ( (element) =>
			{
				Value key;
				Value value;

				key = element.Key.Evaluate (store, writer);
				value = element.Value.Evaluate (store, writer);

				return new KeyValuePair<Value, Value> (key, value);
			}));
		}

		public override string ToString ()
		{
			StringBuilder builder;
			bool comma;

			builder = new StringBuilder ();
			builder.Append ('[');

			comma = false;

			foreach (KeyValuePair<IEvaluator, IEvaluator> element in this.elements)
			{
				if (comma)
					builder.Append (", ");
				else
					comma = true;

				builder.Append (element.Key);
				builder.Append (": ");
				builder.Append (element.Value);
			}

			builder.Append (']');

			return builder.ToString ();
		}

		#endregion
	}
}
