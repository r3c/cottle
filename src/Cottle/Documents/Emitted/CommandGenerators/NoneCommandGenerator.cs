namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class NoneCommandGenerator : ICommandGenerator
    {
        public bool Generate(Emitter emitter)
        {
            return false;
        }
    }
}