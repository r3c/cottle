namespace Cottle.Documents.Default
{
    public readonly struct Stack
    {
        public readonly Value[] Globals;
        public readonly Value[] Locals;

        public Stack(Value[] globals, int localCount)
        {
            Globals = globals;
            Locals = localCount > 0 ? new Value[localCount] : null;
        }
    }
}