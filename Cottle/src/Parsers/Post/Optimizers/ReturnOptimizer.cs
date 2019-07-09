namespace Cottle.Parsers.Post.Optimizers
{
    /// <summary>
    /// Remove all commands following "return" in a composite command.
    /// </summary>
    internal class ReturnOptimizer : AbstractOptimizer
    {
        public static readonly ReturnOptimizer Instance = new ReturnOptimizer();

        public override Command Optimize(Command command)
        {
            if (command.Type == CommandType.Composite && command.Body != null &&
                command.Body.Type == CommandType.Return)
                return command.Body;

            return command;
        }
    }
}