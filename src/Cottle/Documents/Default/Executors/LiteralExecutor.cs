using System.IO;
using System.Text;
using Cottle.Values;

namespace Cottle.Documents.Default.Executors
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