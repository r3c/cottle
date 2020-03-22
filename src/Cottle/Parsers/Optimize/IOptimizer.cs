namespace Cottle.Parsers.Optimize
{
    internal interface IOptimizer
    {
        Statement Optimize(Statement statement);

        Expression Optimize(Expression expression);
    }
}