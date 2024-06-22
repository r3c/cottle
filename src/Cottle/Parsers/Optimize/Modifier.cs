using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Contexts;

namespace Cottle.Parsers.Optimize
{
    internal static class Modifier
    {
        public static readonly IReadOnlyList<Func<Expression, Expression>> ExpressionModifiers = new Func<Expression, Expression>[]
        {
            // Fold constant access expressions into constant expressions
            expression =>
            {
                if (expression.Type != ExpressionType.Access || expression.Subscript.Type != ExpressionType.Constant)
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
                            if (element.Key.Type != ExpressionType.Constant || !Modifier.IsPure(element.Value))
                                unsure = true;
                            else if (element.Key.Value == subscript)
                                value = element.Value;
                        }

                        return unsure ? expression : value;

                    default:
                        return expression;
                }
            },

            // Evaluate constant invoke expressions on pure functions into constant expressions
            expression =>
            {
                if (expression.Type != ExpressionType.Invoke || expression.Source.Type != ExpressionType.Constant)
                    return expression;

                var source = expression.Source.Value;

                if (source.Type != ValueContent.Function)
                    return Expression.Void;

                if (!source.AsFunction.IsPure || expression.Arguments.Any(argument => argument.Type != ExpressionType.Constant))
                    return expression;

                var arguments = expression.Arguments.Select(a => a.Value).ToList();
                var result = source.AsFunction.Invoke(null, arguments, TextWriter.Null);

                return Expression.CreateConstant(result);
            },

            // Fold map of constant expressions into constant map expression
            expression =>
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
            },
        };

        public static readonly IReadOnlyList<Func<Statement, Statement>> StatementModifiers = new Func<Statement, Statement>[]
        {
            // Simplify "if" statement with constant conditions
            statement =>
            {
                while (statement.Type == StatementType.If && statement.Operand.Type == ExpressionType.Constant)
                {
                    if (statement.Operand.Value.AsBoolean)
                        return statement.Body;

                    statement = statement.Next;
                }

                return statement;
            },

            // Remove all statements following "return" in a composite statement
            statement =>
            {
                if (statement.Type == StatementType.Composite && statement.Body.Type == StatementType.Return)
                    return statement.Body;

                return statement;
            }
        };

        private static bool IsPure(Expression expression)
        {
            return expression.Type switch
            {
                ExpressionType.Access => Modifier.IsPure(expression.Source) && Modifier.IsPure(expression.Subscript),
                ExpressionType.Constant => expression.Value.Type != ValueContent.Function || expression.Value.AsFunction.IsPure,
                ExpressionType.Invoke => Modifier.IsPure(expression.Source) && expression.Arguments.All(Modifier.IsPure),
                ExpressionType.Map => expression.Elements.All(element => Modifier.IsPure(element.Key) && Modifier.IsPure(element.Value)),
                ExpressionType.Symbol => true,
                _ => throw new ArgumentOutOfRangeException(nameof(expression)),
            };
        }
    }
}