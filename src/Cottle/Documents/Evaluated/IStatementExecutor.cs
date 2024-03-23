using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated
{
    internal interface IStatementExecutor
    {
        Value? Execute(Runtime runtime, Frame frame, TextWriter output);
    }
}