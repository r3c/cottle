using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated
{
    internal interface IExecutor
    {
        bool Execute(Frame frame, TextWriter output, out Value result);
    }
}