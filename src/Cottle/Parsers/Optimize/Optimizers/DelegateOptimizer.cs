using System;

namespace Cottle.Parsers.Optimize.Optimizers
{
    internal class DelegateOptimizer : IOptimizer
    {
        private readonly Func<Expression, Expression> _expressionOptimizer;

        private readonly Func<Statement, Statement> _statementOptimizer;

        public DelegateOptimizer(Func<Statement, Statement> statementOptimizer, Func<Expression, Expression> expressionOptimizer)
        {
            _expressionOptimizer = expressionOptimizer;
            _statementOptimizer = statementOptimizer;
        }

        public Expression Optimize(Expression expression)
        {
            return _expressionOptimizer(expression);
        }

        public Statement Optimize(Statement statement)
        {
            return _statementOptimizer(statement);
        }
    }
}