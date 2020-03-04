namespace Cottle.Documents.Emitted.Generators
{
    internal class ExpressionConstantGenerator : IGenerator
    {
        private readonly Value _value;

        public ExpressionConstantGenerator(Value value)
        {
            _value = value;
        }

        public void Generate(Emitter emitter)
        {
            emitter.LoadConstant(_value);
        }
    }
}