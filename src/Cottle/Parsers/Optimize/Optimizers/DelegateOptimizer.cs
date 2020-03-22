using System;

namespace Cottle.Parsers.Optimize.Optimizers
{
    internal class DelegateOptimizer : IOptimizer
    {
        private readonly Func<Expression, Expression> _expressionOptimizer;

        private readonly Func<Statement, Statement> _statementOptimizer;

        public DelegateOptimizer(Func<Statement, Statement> statementOptimizer)
        {
            _expressionOptimizer = e => e;
            _statementOptimizer = statementOptimizer;
        }

        public DelegateOptimizer(Func<Expression, Expression> expressionOptimizer)
        {
            _expressionOptimizer = expressionOptimizer;
            _statementOptimizer = c => c;
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