namespace Cottle.Parsers.Post
{
    internal interface IOptimizer
    {
        #region Methods

        Command Optimize(Command command);

        Expression Optimize(Expression expression);

        #endregion
    }
}