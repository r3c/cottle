namespace Cottle.Parsers.Post.Optimizers
{
    internal abstract class AbstractOptimizer : IOptimizer
    {
        public virtual Command Optimize(Command command)
        {
            return command;
        }

        public virtual Expression Optimize(Expression expression)
        {
            return expression;
        }
    }
}