using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                if (!(expression.Type == ExpressionType.Invoke &&
                      Array.TrueForAll(expression.Arguments, RecursiveOptimizer.IsConstant) &&
                      RecursiveOptimizer.IsConstant(expression.Source) &&
                      expression.Source.Value.Type == ValueContent.Function))
                    return expression;

                using (var writer = new StringWriter())
                {
                    var arguments = expression.Arguments.Select(a => a.Value).ToList();
                    var value = expression.Source.Value;

                    return new Expression
                    {
                        Type = ExpressionType.Constant,
                        Value = value.AsFunction.Invoke(new SimpleStore(), arguments, writer)
                    };
                }
            }),

            // Converts constant map expressions to constant expressions
            new DelegateOptimizer(expression =>
            {
                if (!(expression.Type == ExpressionType.Map &&
                      Array.TrueForAll(expression.Elements, RecursiveOptimizer.IsConstant)))
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

        private static bool IsConstant(Expression expression)
        {
            if (expression.Type != ExpressionType.Constant)
                return false;

            var value = expression.Value;

            switch (value.Type)
            {
                case ValueContent.Function:
                    return value.AsFunction.IsPure;

                default:
                    return true;
            }
        }

        private static bool IsConstant(ExpressionElement element)
        {
            return RecursiveOptimizer.IsConstant(element.Key) && RecursiveOptimizer.IsConstant(element.Value);
        }

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
                {
                    var argument = expression.Arguments[i];

                    expression.Arguments[i] = Optimize(argument);
                }
            }

            if (expression.Elements != null)
            {
                for (var i = 0; i < expression.Elements.Length; ++i)
                {
                    var element = expression.Elements[i];

                    expression.Elements[i] = new ExpressionElement(Optimize(element.Key), Optimize(element.Value));
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