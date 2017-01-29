using System;
using System.Collections.Generic;
using System.Linq;

namespace Cottle.Documents.Dynamic
{
	struct Storage
	{
		public readonly string[] Strings;

		public readonly Value[] Values;

		public Storage (IEnumerable<string> strings, IEnumerable<Value> values)
		{
			this.Strings = strings.ToArray ();
			this.Values = values.ToArray ();
		}
	}
}
