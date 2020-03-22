using System.Collections.Generic;
using System.IO;

namespace Cottle
{
    internal interface IParser
    {
        bool Parse(TextReader reader, out Statement statement, out IEnumerable<DocumentReport> reports);
    }
}