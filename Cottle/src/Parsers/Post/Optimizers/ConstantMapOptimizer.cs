using System;
using System.Collections.Generic;

namespace Cottle.Parsers.Post.Optimizers
{
    /// <summary>
    /// Converts constant map expressions to constant expressions.
    /// </summary>
    internal class ConstantMapOptimizer : AbstractOptimizer
    {
        public static readonly ConstantMapOptimizer Instance = new ConstantMapOptimizer();

        private static readonly Predicate<ExpressionElement> Constants = e =>
            e.Key.Type == ExpressionType.Constant && e.Value.Type == ExpressionType.Constant;

        public override Expression Optimize(Expression expression)
        {
            if (expression.Type != ExpressionType.Map ||
                !Array.TrueForAll(expression.Elements, ConstantMapOptimizer.Constants))
                return expression;

            var pairs = new KeyValuePair<Value, Value>[expression.Elements.Length];

            for (var i = expression.Elements.Length; i-- > 0;)
                pairs[i] = new KeyValuePair<Value, Value>(expression.Elements[i].Key.Value,
                    expression.Elements[i].Value.Value);

            return new Expression
            {
                Type = ExpressionType.Constant,
                Value = pairs
            };
        }
    }
}