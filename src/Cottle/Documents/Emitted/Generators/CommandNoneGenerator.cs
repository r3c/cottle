namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandNoneGenerator : IGenerator
    {
        public void Generate(Emitter emitter)
        {
            emitter.LoadBoolean(false);
        }
    }
}