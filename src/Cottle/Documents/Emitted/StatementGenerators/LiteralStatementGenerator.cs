namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class LiteralStatementGenerator : IStatementGenerator
    {
        private readonly string _text;

        public LiteralStatementGenerator(string text)
        {
            _text = text;
        }

        public bool Generate(Emitter emitter)
        {
            emitter.LoadOutput();
            emitter.LoadString(_text);
            emitter.InvokeTextWriterWriteString();

            return false;
        }
    }
}