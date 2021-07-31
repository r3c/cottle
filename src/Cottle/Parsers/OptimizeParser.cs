using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Contexts;
using Cottle.Parsers.Optimize;
using Cottle.Parsers.Optimize.Optimizers;

namespace Cottle.Parsers
{
    internal class OptimizeParser : IParser
    {
        private static readonly IOptimizer Optimizer = new RecursiveOptimizer(new[]
        {
            // Fold constant access expressions into constant expressions
            new DelegateOptimizer(expression =>
            {
                if (expression.Type != ExpressionType.Access ||
                    expression.Subscript.Type != ExpressionType.Constant)
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
                                !OptimizeParser.IsPure(element.Value))
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
                if (expression.Type != ExpressionType.Invoke ||
                    expression.Source.Type != ExpressionType.Constant)
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
                if (expression.Type != ExpressionType.Map)
                    return expression;

                var elements = expression.Elements;
                var isArray = true;

                for (var i = elements.Count; i-- > 0; )
                {
                    var element = elements[i];

                    // Map cannot be converted if one of its keys or values is not constant
                    if (element.Key.Type != ExpressionType.Constant ||
                        element.Value.Type != ExpressionType.Constant)
                        return expression;

                    // Map is an array as long as its keys are following default numeric indices
                    isArray = isArray &&
                        element.Key.Value.Type == ValueContent.Number &&
                        element.Key.Value.AsNumber == i;
                }

                if (isArray)
                {
                    var values = new Value[elements.Count];

                    for (var i = elements.Count; i-- > 0; )
                        values[i] = elements[i].Value.Value;

                    return Expression.CreateConstant(values);
                }
                else
                {
                    var pairs = new KeyValuePair<Value, Value>[elements.Count];

                    for (var i = elements.Count; i-- > 0; )
                        pairs[i] = new KeyValuePair<Value, Value>(elements[i].Key.Value, elements[i].Value.Value);

                    return Expression.CreateConstant(pairs);
                }
            }),

            // Simplify "if" statement with constant conditions
            new DelegateOptimizer(statement =>
            {
                while (statement.Type == StatementType.If &&
                       statement.Operand.Type == ExpressionType.Constant)
                {
                    if (statement.Operand.Value.AsBoolean)
                        return statement.Body;

                    statement = statement.Next;
                }

                return statement;
            }),

            // Remove all statements following "return" in a composite statement
            new DelegateOptimizer(statement =>
            {
                if (statement.Type == StatementType.Composite &&
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
                    return OptimizeParser.IsPure(expression.Source) && OptimizeParser.IsPure(expression.Subscript);

                case ExpressionType.Constant:
                    return expression.Value.Type != ValueContent.Function || expression.Value.AsFunction.IsPure;

                case ExpressionType.Invoke:
                    return OptimizeParser.IsPure(expression.Source) &&
                           expression.Arguments.All(OptimizeParser.IsPure);

                case ExpressionType.Map:
                    return expression.Elements.All(element =>
                        OptimizeParser.IsPure(element.Key) && OptimizeParser.IsPure(element.Value));

                case ExpressionType.Symbol:
                    return true;

                default:
                    throw new ArgumentOutOfRangeException(nameof(expression));
            }
        }

        private readonly IParser _parser;

        public OptimizeParser(IParser parser)
        {
            _parser = parser;
        }

        public bool Parse(TextReader reader, ParserState state, out Statement statement)
        {
            if (!_parser.Parse(reader, state, out var original))
            {
                statement = Statement.NoOp;

                return false;
            }

            statement = OptimizeParser.Optimizer.Optimize(original);

            return true;
        }
    }
}