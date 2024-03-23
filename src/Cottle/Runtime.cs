namespace Cottle
{
    internal readonly struct Runtime
    {
        public readonly Value[] Globals;

        public Runtime(Value[] globals)
        {
            Globals = globals;
        }
    }
}