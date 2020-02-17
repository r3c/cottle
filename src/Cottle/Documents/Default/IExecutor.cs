using System.IO;

namespace Cottle.Documents.Default
{
    internal interface IExecutor
    {
        bool Execute(Frame frame, TextWriter output, out Value result);
    }
}