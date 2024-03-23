using System.IO;

namespace Cottle.Documents.Emitted
{
    internal delegate bool Execute(Runtime runtime, Frame frame, TextWriter output, out Value result);
}