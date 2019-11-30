using System.IO;

namespace Cottle.Documents.Default
{
    internal interface IExecutor
    {
        bool Execute(Stack stack, TextWriter output, out Value result);
    }
}