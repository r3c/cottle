namespace Cottle.Parsers.Forward
{
    internal readonly struct Operator
    {
        public readonly IFunction Function;

        public readonly int Precedence;

        public Operator(IFunction function, int precedence)
        {
            Function = function;
            Precedence = precedence;
        }
    }
}