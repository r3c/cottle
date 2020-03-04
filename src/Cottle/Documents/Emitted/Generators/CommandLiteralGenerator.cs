namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandLiteralGenerator : IGenerator
    {
        private readonly string _text;

        public CommandLiteralGenerator(string text)
        {
            _text = text;
        }

        public void Generate(Emitter emitter)
        {
            emitter.LoadOutput();
            emitter.LoadString(_text);
            emitter.InvokeTextWriterWriteString();
            emitter.LoadBoolean(false);
        }
    }
}