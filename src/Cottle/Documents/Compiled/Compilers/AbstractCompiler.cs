using System;
using System.Collections.Generic;

namespace Cottle.Documents.Compiled.Compilers
{
    internal abstract class AbstractCompiler<TCompiled, TExpression> : ICompiler<TCompiled>
        where TCompiled : class
        where TExpression : class
    {
        public (TCompiled, IReadOnlyList<Value>, int) Compile(Command command)
        {
            var scope = new Scope(new Dictionary<Value, int>());
            var result = CompileCommand(command, scope);

            return (result, scope.CreateGlobalNames(), scope.LocalCount);
        }

        protected abstract TCompiled CreateCommandAssignFunction(Symbol symbol, int localCount,
            IReadOnlyList<int> arguments, TCompiled body);

        protected abstract TCompiled CreateCommandAssignRender(Symbol symbol, TCompiled body);

        protected abstract TCompiled CreateCommandAssignValue(Symbol symbol, TExpression expression);

        protected abstract TCompiled CreateCommandComposite(IReadOnlyList<TCompiled> commands);

        protected abstract TCompiled CreateCommandDump(TExpression expression);

        protected abstract TCompiled CreateCommandEcho(TExpression expression);

        protected abstract TCompiled CreateCommandFor(TExpression source, int? key, int value, TCompiled body,
            TCompiled empty);

        protected abstract TCompiled CreateCommandIf(IReadOnlyList<KeyValuePair<TExpression, TCompiled>> branches,
            TCompiled fallback);

        protected abstract TCompiled CreateCommandLiteral(string text);

        protected abstract TCompiled CreateCommandNone();

        protected abstract TCompiled CreateCommandReturn(TExpression expression);

        protected abstract TCompiled CreateCommandWhile(TExpression condition, TCompiled body);

        protected abstract TExpression CreateExpressionAccess(TExpression source, TExpression subscript);

        protected abstract TExpression CreateExpressionConstant(Value value);

        protected abstract TExpression CreateExpressionInvoke(TExpression caller, IReadOnlyList<TExpression> arguments);

        protected abstract TExpression CreateExpressionMap(
            IReadOnlyList<KeyValuePair<TExpression, TExpression>> elements);

        protected abstract TExpression CreateExpressionSymbol(Symbol symbol);

        protected abstract TExpression CreateExpressionVoid();

        private TCompiled CompileCommand(Command command, Scope scope)
        {
            switch (command.Type)
            {
                case CommandType.AssignFunction:
                    var functionArguments = new int[command.Arguments.Count];
                    var functionScope = scope.CreateLocalScope();

                    for (var i = 0; i < command.Arguments.Count; ++i)
                        functionArguments[i] = functionScope.DeclareLocal(command.Arguments[i]);

                    var functionBody = CompileCommand(command.Body, functionScope);
                    var localCount = functionScope.LocalCount;

                    var functionSymbol = scope.Resolve(command.Key, command.Mode);

                    return CreateCommandAssignFunction(functionSymbol, localCount, functionArguments, functionBody);

                case CommandType.AssignRender:
                    scope.Enter();

                    var renderBody = CompileCommand(command.Body, scope);

                    scope.Leave();

                    var renderSymbol = scope.Resolve(command.Key, command.Mode);

                    return CreateCommandAssignRender(renderSymbol, renderBody);

                case CommandType.AssignValue:
                    var expression = CompileExpression(command.Operand, scope);
                    var valueSymbol = scope.Resolve(command.Key, command.Mode);

                    return CreateCommandAssignValue(valueSymbol, expression);

                case CommandType.Composite:
                    var nodes = new List<TCompiled>();

                    for (; command.Type == CommandType.Composite; command = command.Next)
                        nodes.Add(CompileCommand(command.Body, scope));

                    nodes.Add(CompileCommand(command, scope));

                    return CreateCommandComposite(nodes);

                case CommandType.Dump:
                    return CreateCommandDump(CompileExpression(command.Operand, scope));

                case CommandType.Echo:
                    return CreateCommandEcho(CompileExpression(command.Operand, scope));

                case CommandType.For:
                    var forSource = CompileExpression(command.Operand, scope);

                    scope.Enter();

                    var forKey = !string.IsNullOrEmpty(command.Key) ? (int?)scope.DeclareLocal(command.Key) : null;
                    var forValue = scope.DeclareLocal(command.Value);

                    var forBody = CompileCommand(command.Body, scope);
                    var forEmpty = command.Next.Type != CommandType.None
                        ? CompileCommand(command.Next, scope)
                        : null;

                    scope.Leave();

                    return CreateCommandFor(forSource, forKey, forValue, forBody, forEmpty);

                case CommandType.If:
                    var ifBranches = new List<KeyValuePair<TExpression, TCompiled>>();

                    for (; command.Type == CommandType.If; command = command.Next)
                    {
                        var condition = CompileExpression(command.Operand, scope);

                        scope.Enter();

                        var body = CompileCommand(command.Body, scope);

                        scope.Leave();

                        ifBranches.Add(new KeyValuePair<TExpression, TCompiled>(condition, body));
                    }

                    TCompiled ifFallback;

                    if (command.Type != CommandType.None)
                    {
                        scope.Enter();

                        ifFallback = CompileCommand(command, scope);

                        scope.Leave();
                    }
                    else
                        ifFallback = null;

                    return CreateCommandIf(ifBranches, ifFallback);

                case CommandType.Literal:
                    return CreateCommandLiteral(command.Value);

                case CommandType.None:
                    return CreateCommandNone();

                case CommandType.Return:
                    return CreateCommandReturn(CompileExpression(command.Operand, scope));

                case CommandType.While:
                    var whileCondition = CompileExpression(command.Operand, scope);

                    scope.Enter();

                    var whileBody = CompileCommand(command.Body, scope);

                    scope.Leave();

                    return CreateCommandWhile(whileCondition, whileBody);

                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }
        }

        private TExpression CompileExpression(Expression expression, Scope scope)
        {
            switch (expression.Type)
            {
                case ExpressionType.Access:
                    return CreateExpressionAccess(CompileExpression(expression.Source, scope),
                        CompileExpression(expression.Subscript, scope));

                case ExpressionType.Constant:
                    return CreateExpressionConstant(expression.Value);

                case ExpressionType.Invoke:
                    var arguments = new TExpression[expression.Arguments.Count];

                    for (var i = 0; i < arguments.Length; ++i)
                        arguments[i] = CompileExpression(expression.Arguments[i], scope);

                    return CreateExpressionInvoke(CompileExpression(expression.Source, scope), arguments);

                case ExpressionType.Map:
                    var elements = new KeyValuePair<TExpression, TExpression>[expression.Elements.Count];

                    for (var i = 0; i < elements.Length; ++i)
                    {
                        var key = CompileExpression(expression.Elements[i].Key, scope);
                        var value = CompileExpression(expression.Elements[i].Value, scope);

                        elements[i] = new KeyValuePair<TExpression, TExpression>(key, value);
                    }

                    return CreateExpressionMap(elements);

                case ExpressionType.Symbol:
                    return CreateExpressionSymbol(scope.Resolve(expression.Value.AsString, StoreMode.Global));

                default:
                    return CreateExpressionVoid();
            }
        }
    }
}