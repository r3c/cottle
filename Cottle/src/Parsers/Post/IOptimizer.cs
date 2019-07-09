namespace Cottle.Parsers.Post
{
    internal interface IOptimizer
    {
        Command Optimize(Command command);

        Expression Optimize(Expression expression);
    }
}