using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Stores;
using Cottle.Values;

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

                var subscript = expression.Subscript.Value;

                switch (expression.Source.Type)
                {
                    case ExpressionType.Constant:
                        return new Expression
                        {
                            Type = ExpressionType.Constant,
                            Value = expression.Source.Value.Fields[subscript]
                        };

                    case ExpressionType.Map:
                        var unresolved = false;

                        foreach (var element in expression.Source.Elements)
                        {
                            if (element.Key.Type != ExpressionType.Constant)
                                unresolved = true;
                            else if (element.Key.Value == subscript)
                                return element.Value;
                        }

                        return unresolved
                            ? expression
                            : new Expression
                            {
                                Type = ExpressionType.Constant,
                                Value = VoidValue.Instance
                            };

                    default:
                        return expression;
                }
            }),

            // Evaluate constant invoke expressions on pure functions into constant expressions
            new DelegateOptimizer(expression =>
            {
                if (!(expression.Type == ExpressionType.Invoke &&
                      expression.Source.Type == ExpressionType.Constant &&
                      expression.Source.Value.Type == ValueContent.Function &&
                      expression.Source.Value.AsFunction.IsPure &&
                      Array.TrueForAll(expression.Arguments, argument => argument.Type == ExpressionType.Constant)))
                    return expression;

                var arguments = expression.Arguments.Select(a => a.Value).ToList();
                var value = expression.Source.Value;

                return new Expression
                {
                    Type = ExpressionType.Constant,
                    Value = value.AsFunction.Invoke(new SimpleStore(), arguments, TextWriter.Null)
                };
            }),

            // Fold constant map expressions into constant expressions
            new DelegateOptimizer(expression =>
            {
                if (!(expression.Type == ExpressionType.Map &&
                      Array.TrueForAll(expression.Elements,
                          element => element.Key.Type == ExpressionType.Constant &&
                                     element.Value.Type == ExpressionType.Constant)))
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