using System.Collections.Generic;
using System.Linq;

namespace Cottle.Documents.Dynamic
{
    internal struct Storage
    {
        public readonly Value[] Constants;

        public Storage(IEnumerable<Value> values)
        {
            Constants = values.ToArray();
        }
    }
}