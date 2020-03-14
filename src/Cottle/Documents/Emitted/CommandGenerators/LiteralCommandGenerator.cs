namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class LiteralCommandGenerator : ICommandGenerator
    {
        private readonly string _text;

        public LiteralCommandGenerator(string text)
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