namespace Cottle.Parsers.Post.Optimizers
{
    /// <summary>
    ///     Simplify "if" command with constant condition.
    /// </summary>
    internal class IfOptimizer : AbstractOptimizer
    {
        #region Attributes

        public static readonly IfOptimizer Instance = new IfOptimizer();

        #endregion

        #region Methods

        public override Command Optimize(Command command)
        {
            while (command != null && command.Type == CommandType.If && command.Operand.Type == ExpressionType.Constant)
            {
                if (command.Operand.Value.AsBoolean)
                    return command.Body;

                command = command.Next;
            }

            return command ?? Command.NoOp;
        }

        #endregion
    }
}