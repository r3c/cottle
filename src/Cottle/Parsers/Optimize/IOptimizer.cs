namespace Cottle.Parsers.Optimize
{
    internal interface IOptimizer
    {
        Command Optimize(Command command);

        Expression Optimize(Expression expression);
    }
}