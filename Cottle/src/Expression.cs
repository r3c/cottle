namespace Cottle
{
    internal class Expression
    {
        #region Attributes / Static

        public static readonly Expression Empty = new Expression
        {
            Type = ExpressionType.Void
        };

        #endregion

        #region Attributes / Instance

        public Expression[] Arguments;

        public ExpressionElement[] Elements;

        public Expression Source;

        public Expression Subscript;

        public ExpressionType Type;

        public Value Value;

        #endregion
    }
}