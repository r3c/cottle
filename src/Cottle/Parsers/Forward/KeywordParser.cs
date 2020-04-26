using System.Collections.Generic;

namespace Cottle.Parsers.Forward
{
    internal delegate bool KeywordParser(ForwardParser parser, out Statement statement,
        out IEnumerable<DocumentReport> reports);
}