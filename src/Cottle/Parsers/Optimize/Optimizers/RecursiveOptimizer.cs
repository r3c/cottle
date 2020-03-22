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
                            if (element.Key.Type != ExpressionType.Constant || !RecursiveOptimizer.IsPure(element.Value))
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
            // Recursively apply optimizations to command components
            switch (command.Type)
            {
                case CommandType.AssignFunction:
                    command = Command.CreateAssignFunction(command.Key, command.Arguments, command.Mode,
                        Optimize(command.Body));

                    break;

                case CommandType.AssignRender:
                    command = Command.CreateAssignRender(command.Key, command.Mode, Optimize(command.Body));

                    break;

                case CommandType.AssignValue:
                    command = Command.CreateAssignValue(command.Key, command.Mode, Optimize(command.Operand));

                    break;

                case CommandType.Composite:
                    command = Command.CreateComposite(Optimize(command.Body), Optimize(command.Next));

                    break;

                case CommandType.Dump:
                    command = Command.CreateDump(Optimize(command.Operand));

                    break;

                case CommandType.Echo:
                    command = Command.CreateEcho(Optimize(command.Operand));

                    break;

                case CommandType.For:
                    command = Command.CreateFor(command.Key, command.Value, Optimize(command.Operand),
                        Optimize(command.Body), Optimize(command.Next));

                    break;

                case CommandType.If:
                    command = Command.CreateIf(Optimize(command.Operand), Optimize(command.Body),
                        Optimize(command.Next));

                    break;

                case CommandType.Literal:
                case CommandType.None:
                    break;

                case CommandType.Return:
                    command = Command.CreateReturn(Optimize(command.Operand));

                    break;

                case CommandType.While:
                    command = Command.CreateWhile(Optimize(command.Operand), Optimize(command.Body));

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }

            // Apply optimizations to command itself
            return _optimizers.Aggregate(command, (current, optimizer) => optimizer.Optimize(current));
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
    }
}