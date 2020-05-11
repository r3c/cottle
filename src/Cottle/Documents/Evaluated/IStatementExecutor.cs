using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated
{
    internal interface IStatementExecutor
    {
        Value? Execute(Frame frame, TextWriter output);
    }
}