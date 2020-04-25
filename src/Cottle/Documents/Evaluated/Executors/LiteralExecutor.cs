using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Executors
{
    internal class LiteralExecutor : IExecutor
    {
        private readonly string _text;

        public LiteralExecutor(string text)
        {
            _text = text;
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            output.Write(_text);

            result = Value.Undefined;

            return false;
        }
    }
}