using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Values;

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

            result = VoidValue.Instance;

            return false;
        }
    }
}