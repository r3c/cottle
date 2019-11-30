using System.Collections.Generic;
using System.Linq;

namespace Cottle.Documents.Dynamic
{
    internal struct DynamicStorage
    {
        public readonly Value[] Constants;

        public DynamicStorage(IEnumerable<Value> values)
        {
            Constants = values.ToArray();
        }
    }
}