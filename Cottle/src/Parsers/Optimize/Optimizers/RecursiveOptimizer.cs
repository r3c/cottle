using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Functions;
using Cottle.Stores;

namespace Cottle.Parsers.Optimize.Optimizers
{
    internal class RecursiveOptimizer : IOptimizer
    {
        public static readonly IOptimizer Instance = new RecursiveOptimizer(new[]
        {
            // Evaluate constant invoke expressions using pure functions
            new DelegateOptimizer(expression =>
            {
                if (expression.Type != ExpressionType.Invoke ||
                    !Array.TrueForAll(expression.Arguments, RecursiveOptimizer.IsExpressionConstant) ||
                    !RecursiveOptimizer.IsExpressionConstant(expression.Source) ||
                    expression.Source.Value.Type != ValueContent.Function)
                    return expression;

                if (!(expression.Source.Value.AsFunction is NativeFunction function) || !function.Pure)
                    return expression;

                return new Expression
                {
                    Type = ExpressionType.Constant,
                    Value = function.Execute(expression.Arguments.Select(a => a.Value).ToList(), new SimpleStore(),
                        new StringWriter())
                };
            }),

            // Converts constant map expressions to constant expressions
            new DelegateOptimizer(expression =>
            {
                if (expression.Type != ExpressionType.Map ||
                    !Array.TrueForAll(expression.Elements, RecursiveOptimizer.IsElementConstant))
                    return expression;

                var pairs = new KeyValuePair<Value, Value>[expression.Elements.Length];

                for (var i = expression.Elements.Length; i-- > 0;)
                {
                    pairs[i] = new KeyValuePair<Value, Value>(expression.Elements[i].Key.Value,
                        expression.Elements[i].Value.Value);
                }

                return new Expression
                {
                    Type = ExpressionType.Constant,
                    Value = pairs
                };
            }),

            // Simplify "if" command with constant conditions
            new DelegateOptimizer(command =>
            {
                while (command != null && command.Type == CommandType.If &&
                       command.Operand.Type == ExpressionType.Constant)
                {
                    if (command.Operand.Value.AsBoolean)
                        return command.Body;

                    command = command.Next;
                }

                return command ?? Command.NoOp;
            }),

            // Remove all commands following "return" in a composite command
            new DelegateOptimizer(command =>
            {
                if (command.Type == CommandType.Composite && command.Body != null &&
                    command.Body.Type == CommandType.Return)
                    return command.Body;

                return command;
            })
        });

        private static readonly Predicate<ExpressionElement> IsElementConstant = e =>
            RecursiveOptimizer.IsExpressionConstant(e.Key) && RecursiveOptimizer.IsExpressionConstant(e.Value);

        private static readonly Predicate<Expression> IsExpressionConstant = e => e.Type == ExpressionType.Constant; 

        private readonly IEnumerable<IOptimizer> _optimizers;

        private RecursiveOptimizer(IEnumerable<IOptimizer> optimizers)
        {
            _optimizers = optimizers;
        }

        public Command Optimize(Command command)
        {
            foreach (var optimizer in _optimizers)
                command = optimizer.Optimize(command);

            if (command.Body != null)
                command.Body = Optimize(command.Body);

            if (command.Next != null)
                command.Next = Optimize(command.Next);

            if (command.Operand != null)
                command.Operand = Optimize(command.Operand);

            return command;
        }

        public Expression Optimize(Expression expression)
        {
            foreach (var optimizer in _optimizers)
                expression = optimizer.Optimize(expression);

            if (expression.Arguments != null)
            {
                for (var i = 0; i < expression.Arguments.Length; ++i)
                    expression.Arguments[i] = Optimize(expression.Arguments[i]);
            }

            if (expression.Elements != null)
            {
                for (var i = 0; i < expression.Elements.Length; ++i)
                {
                    expression.Elements[i] = new ExpressionElement(Optimize(expression.Elements[i].Key),
                        Optimize(expression.Elements[i].Value));
                }
            }

            if (expression.Source != null)
                expression.Source = Optimize(expression.Source);

            if (expression.Subscript != null)
                expression.Subscript = Optimize(expression.Subscript);

            return expression;
        }
    }
}