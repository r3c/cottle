using System.Collections.Generic;
using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents.Emitted
{
    internal delegate bool Execute(IReadOnlyList<Value> constants, Frame frame, TextWriter output, out Value result);
}