namespace Cottle.Benchmark
{
    public readonly struct Input<T>
    {
        public readonly T Value;

        private readonly string _name;

        public Input(string name, T value)
        {
            _name = name;
            Value = value;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}