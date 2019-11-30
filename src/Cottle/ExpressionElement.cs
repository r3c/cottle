namespace Cottle
{
    internal readonly struct ExpressionElement
    {
        public readonly Expression Key;

        public readonly Expression Value;

        public ExpressionElement(Expression key, Expression value)
        {
            Key = key;
            Value = value;
        }
    }
}