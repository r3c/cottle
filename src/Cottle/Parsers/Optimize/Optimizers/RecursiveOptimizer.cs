using System;

namespace Cottle.Parsers.Optimize.Optimizers
{
    internal class RecursiveOptimizer : IOptimizer
    {
        private readonly Func<Expression, Expression> _optimizeExpression;
        private readonly Func<Statement, Statement> _optimizeStatement;

        public RecursiveOptimizer(Func<Statement, Statement> optimizeStatement, Func<Expression, Expression> optimizeExpression)
        {
            _optimizeExpression = optimizeExpression;
            _optimizeStatement = optimizeStatement;
        }

        public Expression Optimize(Expression expression)
        {
            // Recursively apply optimizations to expression components
            switch (expression.Type)
            {
                case ExpressionType.Access:
                    expression = Expression.CreateAccess(Optimize(expression.Source), Optimize(expression.Subscript));

                    break;

                case ExpressionType.Constant:
                    expression = Expression.CreateConstant(expression.Value);

                    break;

                case ExpressionType.Invoke:
                    var arguments = new Expression[expression.Arguments.Count];

                    for (var i = 0; i < expression.Arguments.Count; ++i)
                        arguments[i] = Optimize(expression.Arguments[i]);

                    expression = Expression.CreateInvoke(Optimize(expression.Source), arguments);

                    break;

                case ExpressionType.Map:
                    var elements = new ExpressionElement[expression.Elements.Count];

                    for (var i = 0; i < expression.Elements.Count; ++i)
                    {
                        var element = expression.Elements[i];

                        elements[i] = new ExpressionElement(Optimize(element.Key), Optimize(element.Value));
                    }

                    expression = Expression.CreateMap(elements);

                    break;

                case ExpressionType.Symbol:
                    expression = Expression.CreateSymbol(expression.Value.AsString);

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(expression));
            }

            // Apply child optimization to expression itself
            return _optimizeExpression(expression);
        }

        public Statement Optimize(Statement statement)
        {
            // Recursively apply optimizations to statement components
            switch (statement.Type)
            {
                case StatementType.AssignFunction:
                    statement = Statement.CreateAssignFunction(statement.Key, statement.Arguments, statement.Mode,
                        Optimize(statement.Body));

                    break;

                case StatementType.AssignRender:
                    statement = Statement.CreateAssignRender(statement.Key, statement.Mode, Optimize(statement.Body));

                    break;

                case StatementType.AssignValue:
                    statement = Statement.CreateAssignValue(statement.Key, statement.Mode, Optimize(statement.Operand));

                    break;

                case StatementType.Composite:
                    statement = Statement.CreateComposite(Optimize(statement.Body), Optimize(statement.Next));

                    break;

                case StatementType.Dump:
                    statement = Statement.CreateDump(Optimize(statement.Operand));

                    break;

                case StatementType.Echo:
                    statement = Statement.CreateEcho(Optimize(statement.Operand));

                    break;

                case StatementType.For:
                    statement = Statement.CreateFor(statement.Key, statement.Value, Optimize(statement.Operand),
                        Optimize(statement.Body), Optimize(statement.Next));

                    break;

                case StatementType.If:
                    statement = Statement.CreateIf(Optimize(statement.Operand), Optimize(statement.Body),
                        Optimize(statement.Next));

                    break;

                case StatementType.Literal:
                case StatementType.None:
                    break;

                case StatementType.Return:
                    statement = Statement.CreateReturn(Optimize(statement.Operand));

                    break;

                case StatementType.Unwrap:
                    statement = Statement.CreateUnwrap(Optimize(statement.Body));

                    break;

                case StatementType.While:
                    statement = Statement.CreateWhile(Optimize(statement.Operand), Optimize(statement.Body));

                    break;

                case StatementType.Wrap:
                    statement = Statement.CreateWrap(Optimize(statement.Operand), Optimize(statement.Body));

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(statement));
            }

            // Apply child optimization to statement itself
            return _optimizeStatement(statement);
        }
    }
}