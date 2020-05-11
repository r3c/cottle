using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class LiteralStatementExecutor : IStatementExecutor
    {
        private readonly string _text;

        public LiteralStatementExecutor(string text)
        {
            _text = text;
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            output.Write(_text);

            return null;
        }
    }
}