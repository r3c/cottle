using System;
using System.IO;
using System.Linq;
using Cottle.Functions;
using Cottle.Stores;

namespace Cottle.Parsers.Post.Optimizers
{
    /// <summary>
    /// Evaluate constant invoke expressions using pure functions.
    /// </summary>
    internal class ConstantInvokeOptimizer : AbstractOptimizer
    {
        public static readonly ConstantInvokeOptimizer Instance = new ConstantInvokeOptimizer();

        public override Expression Optimize(Expression expression)
        {
            if (expression.Type != ExpressionType.Invoke ||
                !Array.TrueForAll(expression.Arguments, a => a.Type == ExpressionType.Constant) ||
                expression.Source.Type != ExpressionType.Constant ||
                expression.Source.Value.Type != ValueContent.Function)
                return expression;

            var function = expression.Source.Value.AsFunction as NativeFunction;

            if (function == null || !function.Pure)
                return expression;

            return new Expression
            {
                Type = ExpressionType.Constant,
                Value = function.Execute(expression.Arguments.Select(a => a.Value).ToList(), new SimpleStore(),
                    new StringWriter())
            };
        }
    }
}