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

                var source = expression.Source;
                var subscript = expression.Subscript.Value;

                switch (source.Type)
                {
                    case ExpressionType.Constant:
                        return Expression.CreateConstant(source.Value.Fields[subscript]);

                    case ExpressionType.Map:
                        var impure = false;
                        var value = Expression.Void;

                        foreach (var element in source.Elements)
                        {
                            if (!RecursiveOptimizer.IsPure(element.Key) || !RecursiveOptimizer.IsPure(element.Value))
                                impure = true;
                            else if (element.Key.Type == ExpressionType.Constant && element.Key.Value == subscript)
                                value = element.Value;
                        }

                        return impure ? expression : value;

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

        public Command Optimize(Command command)
        {
            foreach (var optimizer in _optimizers)
                command = optimizer.Optimize(command);

            switch (command.Type)
            {
                case CommandType.AssignFunction:
                    return Command.CreateAssignFunction(command.Key, command.Arguments, command.Mode,
                        Optimize(command.Body));

                case CommandType.AssignRender:
                    return Command.CreateAssignRender(command.Key, command.Mode, Optimize(command.Body));

                case CommandType.AssignValue:
                    return Command.CreateAssignValue(command.Key, command.Mode, Optimize(command.Operand));

                case CommandType.Composite:
                    return Command.CreateComposite(Optimize(command.Body), Optimize(command.Next));

                case CommandType.Dump:
                    return Command.CreateDump(Optimize(command.Operand));

                case CommandType.Echo:
                    return Command.CreateEcho(Optimize(command.Operand));

                case CommandType.For:
                    return Command.CreateFor(command.Key, command.Value, Optimize(command.Operand),
                        Optimize(command.Body), Optimize(command.Next));

                case CommandType.If:
                    return Command.CreateIf(Optimize(command.Operand), Optimize(command.Body), Optimize(command.Next));

                case CommandType.Literal:
                case CommandType.None:
                    return command;

                case CommandType.Return:
                    return Command.CreateReturn(Optimize(command.Operand));

                case CommandType.While:
                    return Command.CreateWhile(Optimize(command.Operand), Optimize(command.Body));

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }
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
                    return Expression.CreateSymbol(expression.Value.AsString);

                default:
                    throw new ArgumentOutOfRangeException(nameof(expression));
            }
        }
    }
}