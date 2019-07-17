using System;

namespace Cottle.Parsers.Optimize.Optimizers
{
    internal class DelegateOptimizer : IOptimizer
    {
        private readonly Func<Command, Command> _commandOptimizer;

        private readonly Func<Expression, Expression> _expressionOptimizer;

        public DelegateOptimizer(Func<Command, Command> commandOptimizer)
        {
            _commandOptimizer = commandOptimizer;
            _expressionOptimizer = e => e;
        }

        public DelegateOptimizer(Func<Expression, Expression> expressionOptimizer)
        {
            _commandOptimizer = c => c;
            _expressionOptimizer = expressionOptimizer;
        }

        public Command Optimize(Command command)
        {
            return _commandOptimizer(command);
        }

        public Expression Optimize(Expression expression)
        {
            return _expressionOptimizer(expression);
        }
    }
}