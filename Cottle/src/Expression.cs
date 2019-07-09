namespace Cottle
{
    internal class Expression
    {
        public static readonly Expression Empty = new Expression
        {
            Type = ExpressionType.Void
        };

        public Expression[] Arguments;

        public ExpressionElement[] Elements;

        public Expression Source;

        public Expression Subscript;

        public ExpressionType Type;

        public Value Value;
    }
}