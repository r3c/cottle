using System.Collections.Generic;
using System.IO;

namespace Cottle.Documents.Emitted
{
    internal delegate bool Execute(IReadOnlyList<Value> constants, Frame frame, TextWriter output, out Value result);
}