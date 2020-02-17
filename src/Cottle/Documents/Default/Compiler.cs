using System;
using System.Collections.Generic;
using Cottle.Documents.Default.Evaluators;
using Cottle.Documents.Default.Executors;
using Cottle.Documents.Default.Executors.Assign;

namespace Cottle.Documents.Default
{
    internal static class Compiler
    {
        public static (IExecutor, IReadOnlyList<Value>, int) Compile(Command command)
        {
            var scope = new Scope(new Dictionary<Value, int>());
            var node = Compiler.CompileCommand(command, scope);

            return (node, scope.CreateGlobalNames(), scope.LocalCount);
        }

        private static IExecutor CompileCommand(Command command, Scope scope)
        {
            switch (command.Type)
            {
                case CommandType.AssignFunction:
                    var functionArguments = new int[command.Arguments.Count];
                    var functionScope = scope.CreateLocalScope();

                    for (var i = 0; i < command.Arguments.Count; ++i)
                        functionArguments[i] = functionScope.DeclareLocal(command.Arguments[i]);

                    var functionBody = Compiler.CompileCommand(command.Body, functionScope);
                    var localCount = functionScope.LocalCount;

                    var functionSymbol = scope.Resolve(command.Key, command.Mode);

                    return new FunctionAssignExecutor(functionSymbol, localCount, functionArguments, functionBody);

                case CommandType.AssignRender:
                    scope.Enter();

                    var renderBody = Compiler.CompileCommand(command.Body, scope);

                    scope.Leave();

                    var renderSymbol = scope.Resolve(command.Key, command.Mode);

                    return new RenderAssignExecutor(renderSymbol, renderBody);

                case CommandType.AssignValue:
                    var expression = Compiler.CompileExpression(command.Operand, scope);
                    var valueSymbol = scope.Resolve(command.Key, command.Mode);

                    return new ValueAssignExecutor(valueSymbol, expression);

                case CommandType.Composite:
                    var nodes = new List<IExecutor>();

                    for (; command.Type == CommandType.Composite; command = command.Next)
                        nodes.Add(Compiler.CompileCommand(command.Body, scope));

                    nodes.Add(Compiler.CompileCommand(command, scope));

                    return new CompositeExecutor(nodes);

                case CommandType.Dump:
                    return new DumpExecutor(Compiler.CompileExpression(command.Operand, scope));

                case CommandType.Echo:
                    return new EchoExecutor(Compiler.CompileExpression(command.Operand, scope));

                case CommandType.For:
                    var forSource = Compiler.CompileExpression(command.Operand, scope);

                    scope.Enter();

                    var forKey = !string.IsNullOrEmpty(command.Key) ? (int?)scope.DeclareLocal(command.Key) : null;
                    var forValue = scope.DeclareLocal(command.Value);

                    var forBody = Compiler.CompileCommand(command.Body, scope);
                    var forEmpty = command.Next.Type != CommandType.None
                        ? Compiler.CompileCommand(command.Next, scope)
                        : null;

                    scope.Leave();

                    return new ForExecutor(forSource, forKey, forValue, forBody, forEmpty);

                case CommandType.If:
                    var ifBranches = new List<KeyValuePair<IEvaluator, IExecutor>>();

                    for (; command.Type == CommandType.If; command = command.Next)
                    {
                        var condition = Compiler.CompileExpression(command.Operand, scope);

                        scope.Enter();

                        var body = Compiler.CompileCommand(command.Body, scope);

                        scope.Leave();

                        ifBranches.Add(new KeyValuePair<IEvaluator, IExecutor>(condition, body));
                    }

                    IExecutor ifFallback;

                    if (command.Type != CommandType.None)
                    {
                        scope.Enter();

                        ifFallback = Compiler.CompileCommand(command, scope);

                        scope.Leave();
                    }
                    else
                        ifFallback = null;

                    return new IfExecutor(ifBranches, ifFallback);

                case CommandType.Literal:
                    return new LiteralExecutor(command.Value);

                case CommandType.None:
                    return new LiteralExecutor(string.Empty);

                case CommandType.Return:
                    return new ReturnExecutor(Compiler.CompileExpression(command.Operand, scope));

                case CommandType.While:
                    var whileCondition = Compiler.CompileExpression(command.Operand, scope);

                    scope.Enter();

                    var whileBody = Compiler.CompileCommand(command.Body, scope);

                    scope.Leave();

                    return new WhileExecutor(whileCondition, whileBody);

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }
        }

        private static IEvaluator CompileExpression(Expression expression, Scope scope)
        {
            switch (expression.Type)
            {
                case ExpressionType.Access:
                    return new AccessEvaluator(Compiler.CompileExpression(expression.Source, scope),
                        Compiler.CompileExpression(expression.Subscript, scope));

                case ExpressionType.Constant:
                    return new ConstantEvaluator(expression.Value);

                case ExpressionType.Invoke:
                    var arguments = new IEvaluator[expression.Arguments.Count];

                    for (var i = 0; i < arguments.Length; ++i)
                        arguments[i] = Compiler.CompileExpression(expression.Arguments[i], scope);

                    return new InvokeEvaluator(Compiler.CompileExpression(expression.Source, scope), arguments);

                case ExpressionType.Map:
                    var elements = new KeyValuePair<IEvaluator, IEvaluator>[expression.Elements.Count];

                    for (var i = 0; i < elements.Length; ++i)
                    {
                        var key = Compiler.CompileExpression(expression.Elements[i].Key, scope);
                        var value = Compiler.CompileExpression(expression.Elements[i].Value, scope);

                        elements[i] = new KeyValuePair<IEvaluator, IEvaluator>(key, value);
                    }

                    return new MapEvaluator(elements);

                case ExpressionType.Symbol:
                    return new SymbolEvaluator(scope.Resolve(expression.Value.AsString, StoreMode.Global));

                default:
                    return VoidEvaluator.Instance;
            }
        }
    }
}