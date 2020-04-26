using System;
using System.Collections.Generic;

namespace Cottle.Documents.Compiled.Compilers
{
    internal abstract class AbstractCompiler<TAssembly, TExpression> : ICompiler<TAssembly>
        where TAssembly : class
        where TExpression : class
    {
        public (TAssembly, IReadOnlyList<Value>, int) Compile(Statement statement)
        {
            var scope = new Scope(new Dictionary<Value, int>());
            var result = CompileStatement(statement, scope);

            return (result, scope.CreateGlobalNames(), scope.LocalCount);
        }

        protected abstract TExpression CreateExpressionAccess(TExpression source, TExpression subscript);

        protected abstract TExpression CreateExpressionConstant(Value value);

        protected abstract TExpression CreateExpressionInvoke(TExpression caller, IReadOnlyList<TExpression> arguments);

        protected abstract TExpression CreateExpressionMap(
            IReadOnlyList<KeyValuePair<TExpression, TExpression>> elements);

        protected abstract TExpression CreateExpressionSymbol(Symbol symbol);

        protected abstract TExpression CreateExpressionVoid();

        protected abstract TAssembly CreateStatementAssignFunction(Symbol symbol, int localCount,
            IReadOnlyList<Symbol> arguments, TAssembly body);

        protected abstract TAssembly CreateStatementAssignRender(Symbol symbol, TAssembly body);

        protected abstract TAssembly CreateStatementAssignValue(Symbol symbol, TExpression expression);

        protected abstract TAssembly CreateStatementComposite(IReadOnlyList<TAssembly> statements);

        protected abstract TAssembly CreateStatementDump(TExpression expression);

        protected abstract TAssembly CreateStatementEcho(TExpression expression);

        protected abstract TAssembly CreateStatementFor(TExpression source, Symbol? key, Symbol value, TAssembly body,
            TAssembly empty);

        protected abstract TAssembly CreateStatementIf(IReadOnlyList<KeyValuePair<TExpression, TAssembly>> branches,
            TAssembly fallback);

        protected abstract TAssembly CreateStatementLiteral(string text);

        protected abstract TAssembly CreateStatementNone();

        protected abstract TAssembly CreateStatementReturn(TExpression expression);

        protected abstract TAssembly CreateStatementUnwrap(TAssembly body);

        protected abstract TAssembly CreateStatementWhile(TExpression condition, TAssembly body);

        protected abstract TAssembly CreateStatementWrap(TExpression modifier, TAssembly body);

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
                    return CreateExpressionSymbol(scope.GetOrDeclareClosest(expression.Value.AsString, StoreMode.Global));

                default:
                    return CreateExpressionVoid();
            }
        }

        private TAssembly CompileStatement(Statement statement, Scope scope)
        {
            switch (statement.Type)
            {
                case StatementType.AssignFunction:
                    var functionArguments = new Symbol[statement.Arguments.Count];
                    var functionScope = scope.CreateLocalScope();

                    for (var i = 0; i < statement.Arguments.Count; ++i)
                        functionArguments[i] = functionScope.GetOrDeclareLocal(statement.Arguments[i]);

                    var functionBody = CompileStatement(statement.Body, functionScope);
                    var localCount = functionScope.LocalCount;

                    var functionSymbol = scope.GetOrDeclareClosest(statement.Key, statement.Mode);

                    return CreateStatementAssignFunction(functionSymbol, localCount, functionArguments, functionBody);

                case StatementType.AssignRender:
                    scope.Enter();

                    var renderBody = CompileStatement(statement.Body, scope);

                    scope.Leave();

                    var renderSymbol = scope.GetOrDeclareClosest(statement.Key, statement.Mode);

                    return CreateStatementAssignRender(renderSymbol, renderBody);

                case StatementType.AssignValue:
                    var assignValueExpression = CompileExpression(statement.Operand, scope);
                    var assignValueSymbol = scope.GetOrDeclareClosest(statement.Key, statement.Mode);

                    return CreateStatementAssignValue(assignValueSymbol, assignValueExpression);

                case StatementType.Composite:
                    var nodes = new List<TAssembly>();

                    for (; statement.Type == StatementType.Composite; statement = statement.Next)
                        nodes.Add(CompileStatement(statement.Body, scope));

                    nodes.Add(CompileStatement(statement, scope));

                    return CreateStatementComposite(nodes);

                case StatementType.Dump:
                    return CreateStatementDump(CompileExpression(statement.Operand, scope));

                case StatementType.Echo:
                    return CreateStatementEcho(CompileExpression(statement.Operand, scope));

                case StatementType.For:
                    var forSource = CompileExpression(statement.Operand, scope);

                    scope.Enter();

                    var forKey = !string.IsNullOrEmpty(statement.Key)
                        ? (Symbol?)scope.GetOrDeclareLocal(statement.Key)
                        : null;
                    var forValue = scope.GetOrDeclareLocal(statement.Value);

                    var forBody = CompileStatement(statement.Body, scope);
                    var forEmpty = statement.Next.Type != StatementType.None
                        ? CompileStatement(statement.Next, scope)
                        : null;

                    scope.Leave();

                    return CreateStatementFor(forSource, forKey, forValue, forBody, forEmpty);

                case StatementType.If:
                    var ifBranches = new List<KeyValuePair<TExpression, TAssembly>>();

                    for (; statement.Type == StatementType.If; statement = statement.Next)
                    {
                        var condition = CompileExpression(statement.Operand, scope);

                        scope.Enter();

                        var body = CompileStatement(statement.Body, scope);

                        scope.Leave();

                        ifBranches.Add(new KeyValuePair<TExpression, TAssembly>(condition, body));
                    }

                    TAssembly ifFallback;

                    if (statement.Type != StatementType.None)
                    {
                        scope.Enter();

                        ifFallback = CompileStatement(statement, scope);

                        scope.Leave();
                    }
                    else
                        ifFallback = null;

                    return CreateStatementIf(ifBranches, ifFallback);

                case StatementType.Literal:
                    return CreateStatementLiteral(statement.Value);

                case StatementType.None:
                    return CreateStatementNone();

                case StatementType.Return:
                    return CreateStatementReturn(CompileExpression(statement.Operand, scope));

                case StatementType.Unwrap:
                    scope.Enter();

                    var unwrapBody = CompileStatement(statement.Body, scope);

                    scope.Leave();

                    return CreateStatementUnwrap(unwrapBody);

                case StatementType.While:
                    var whileCondition = CompileExpression(statement.Operand, scope);

                    scope.Enter();

                    var whileBody = CompileStatement(statement.Body, scope);

                    scope.Leave();

                    return CreateStatementWhile(whileCondition, whileBody);

                case StatementType.Wrap:
                    var wrapModifier = CompileExpression(statement.Operand, scope);

                    scope.Enter();

                    var wrapBody = CompileStatement(statement.Body, scope);

                    scope.Leave();

                    return CreateStatementWrap(wrapModifier, wrapBody);

                default:
                    throw new ArgumentOutOfRangeException(nameof(statement));
            }
        }
    }
}