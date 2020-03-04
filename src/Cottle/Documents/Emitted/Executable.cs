using System.Collections.Generic;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted
{
    internal delegate bool Executable(IReadOnlyList<Value> constants, Frame frame, TextWriter output, out Value result);
}