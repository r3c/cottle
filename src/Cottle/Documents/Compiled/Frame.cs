namespace Cottle.Documents.Compiled
{
    internal readonly struct Frame
    {
        public readonly Value[] Globals;
        public readonly Value[] Locals;

        public Frame(Value[] globals, int localCount)
        {
            Globals = globals;
            Locals = localCount > 0 ? new Value[localCount] : null;
        }
    }
}