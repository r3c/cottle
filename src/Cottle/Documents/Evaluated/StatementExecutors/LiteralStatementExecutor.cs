using System.IO;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class LiteralStatementExecutor : IStatementExecutor
    {
        private readonly string _text;

        public LiteralStatementExecutor(string text)
        {
            _text = text;
        }

        public Value? Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            output.Write(_text);

            return null;
        }
    }
}