using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Contexts;

namespace Cottle.Parsers.Optimize.Optimizers
{
    internal class RecursiveOptimizer : IOptimizer
    {
        public static readonly IOptimizer Instance = new RecursiveOptimizer(new[]
        {
            // Fold constant access expressions into constant expressions
            new DelegateOptimizer(expression =>
            {
                if (!(expression.Type == ExpressionType.Access &&
                      expression.Subscript.Type == ExpressionType.Constant))
                    return expression;

                var source = expression.Source;
                var subscript = expression.Subscript.Value;

                switch (source.Type)
                {
                    case ExpressionType.Constant:
                        return Expression.CreateConstant(source.Value.Fields[subscript]);

                    case ExpressionType.Map:
                        var unsure = false;
                        var value = Expression.Void;

                        foreach (var element in source.Elements)
                        {
                            if (element.Key.Type != ExpressionType.Constant ||
                                !RecursiveOptimizer.IsPure(element.Value))
                                unsure = true;
                            else if (element.Key.Value == subscript)
                                value = element.Value;
                        }

                        return unsure ? expression : value;

                    default:
                        return expression;
                }
            }),

            // Evaluate constant invoke expressions on pure functions into constant expressions
            new DelegateOptimizer(expression =>
            {
                if (!(expression.Type == ExpressionType.Invoke &&
                      expression.Source.Type == ExpressionType.Constant))
                    return expression;

                var source = expression.Source.Value;

                if (source.Type != ValueContent.Function)
                    return Expression.Void;

                if (!(source.AsFunction.IsPure &&
                      expression.Arguments.All(argument => argument.Type == ExpressionType.Constant)))
                    return expression;

                var arguments = expression.Arguments.Select(a => a.Value).ToList();
                var result = source.AsFunction.Invoke(EmptyContext.Instance, arguments, TextWriter.Null);

                return Expression.CreateConstant(result);
            }),

            // Fold map of constant expressions into constant map expression
            new DelegateOptimizer(expression =>
            {
                if (!(expression.Type == ExpressionType.Map &&
                      expression.Elements.All(element =>
                          element.Key.Type == ExpressionType.Constant &&
                          element.Value.Type == ExpressionType.Constant)))
                    return expression;

                var pairs = new KeyValuePair<Value, Value>[expression.Elements.Count];

                for (var i = expression.Elements.Count; i-- > 0;)
                {
                    var element = expression.Elements[i];

                    pairs[i] = new KeyValuePair<Value, Value>(element.Key.Value, element.Value.Value);
                }

                return Expression.CreateConstant(pairs);
            }),

            // Simplify "if" statement with constant conditions
            new DelegateOptimizer(statement =>
            {
                while (statement != null && statement.Type == StatementType.If &&
                       statement.Operand.Type == ExpressionType.Constant)
                {
                    if (statement.Operand.Value.AsBoolean)
                        return statement.Body;

                    statement = statement.Next;
                }

                return statement ?? Statement.NoOp;
            }),

            // Remove all statements following "return" in a composite statement
            new DelegateOptimizer(statement =>
            {
                if (statement.Type == StatementType.Composite && statement.Body != null &&
                    statement.Body.Type == StatementType.Return)
                    return statement.Body;

                return statement;
            })
        });

        private static bool IsPure(Expression expression)
        {
            switch (expression.Type)
            {
                case ExpressionType.Access:
                    return RecursiveOptimizer.IsPure(expression.Source) &&
                           RecursiveOptimizer.IsPure(expression.Subscript);

                case ExpressionType.Constant:
                    return expression.Value.Type != ValueContent.Function || expression.Value.AsFunction.IsPure;

                case ExpressionType.Invoke:
                    return RecursiveOptimizer.IsPure(expression.Source) &&
                           expression.Arguments.All(RecursiveOptimizer.IsPure);

                case ExpressionType.Map:
                    return expression.Elements.All(element =>
                        RecursiveOptimizer.IsPure(element.Key) && RecursiveOptimizer.IsPure(element.Value));

                case ExpressionType.Symbol:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(expression));
            }
        }

        private readonly IEnumerable<IOptimizer> _optimizers;

        private RecursiveOptimizer(IEnumerable<IOptimizer> optimizers)
        {
            _optimizers = optimizers;
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

            // Apply optimizations to expression itself
            return _optimizers.Aggregate(expression, (current, optimizer) => optimizer.Optimize(current));
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

            // Apply optimizations to statement itself
            return _optimizers.Aggregate(statement, (current, optimizer) => optimizer.Optimize(current));
        }
    }
}