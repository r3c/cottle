using System.Collections.Generic;
using System.IO;

namespace Cottle
{
    internal interface IParser
    {
        bool Parse(TextReader reader, out Command command, out IEnumerable<DocumentReport> reports);
    }
}