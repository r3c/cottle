namespace Cottle.Documents.Emitted.ExpressionGenerators
{
    internal class ConstantExpressionGenerator : IExpressionGenerator
    {
        private readonly Value _value;

        public ConstantExpressionGenerator(Value value)
        {
            _value = value;
        }

        public void Generate(Emitter emitter)
        {
            emitter.EmitLoadConstant(_value);
        }
    }
}