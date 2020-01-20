using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Contexts;
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
                        return Expression.CreateConstant(expression.Source.Value.Fields[subscript]);

                    case ExpressionType.Map:
                        var unresolved = false;

                        foreach (var element in expression.Source.Elements)
                        {
                            if (element.Key.Type != ExpressionType.Constant)
                                unresolved = true;
                            else if (element.Key.Value == subscript)
                                return element.Value;
                        }

                        return unresolved ? expression : Expression.CreateConstant(VoidValue.Instance);

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
                      expression.Arguments.All(argument => argument.Type == ExpressionType.Constant)))
                    return expression;

                var arguments = expression.Arguments.Select(a => a.Value).ToList();
                var source = expression.Source.Value;
                var result = source.AsFunction.Invoke(EmptyContext.Instance, arguments, TextWriter.Null);

                return Expression.CreateConstant(result);
            }),

            // Fold constant map expressions into constant expressions
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
                    pairs[i] = new KeyValuePair<Value, Value>(expression.Elements[i].Key.Value,
                        expression.Elements[i].Value.Value);
                }

                return Expression.CreateConstant(pairs);
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

            switch (expression.Type)
            {
                case ExpressionType.Access:
                    return Expression.CreateAccess(Optimize(expression.Source), Optimize(expression.Subscript));

                case ExpressionType.Constant:
                    return Expression.CreateConstant(expression.Value);

                case ExpressionType.Invoke:
                    var arguments = new Expression[expression.Arguments.Count];

                    for (var i = 0; i < expression.Arguments.Count; ++i)
                        arguments[i] = Optimize(expression.Arguments[i]);

                    return Expression.CreateInvoke(Optimize(expression.Source), arguments);

                case ExpressionType.Map:
                    var elements = new ExpressionElement[expression.Elements.Count];

                    for (var i = 0; i < expression.Elements.Count; ++i)
                    {
                        var element = expression.Elements[i];

                        elements[i] = new ExpressionElement(Optimize(element.Key), Optimize(element.Value));
                    }

                    return Expression.CreateMap(elements);

                case ExpressionType.Symbol:
                    return Expression.CreateSymbol(expression.Value);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expression));
            }
        }
    }
}