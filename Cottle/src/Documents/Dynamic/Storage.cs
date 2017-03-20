using System;
using System.Collections.Generic;
using System.Linq;

namespace Cottle.Documents.Dynamic
{
	struct Storage
	{
		public readonly Value[] Constants;

		public Storage (IEnumerable<Value> values)
		{
			this.Constants = values.ToArray ();
		}
	}
}
