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
            var state = new Allocator(new Dictionary<Value, int>(), true);
            var node = Compiler.CompileCommand(command, state);

            return (node, state.GetGlobals(), state.GetLocalCount());
        }

        private static IExecutor CompileCommand(Command command, Allocator allocator)
        {
            switch (command.Type)
            {
                case CommandType.AssignFunction:
                    var functionState = allocator.EnterFunction();
                    var functionArguments = new int[command.Arguments.Length];

                    for (var i = 0; i < command.Arguments.Length; ++i)
                        functionArguments[i] = functionState.DeclareAsLocal(command.Arguments[i]);

                    var functionBody = Compiler.CompileCommand(command.Body, functionState);
                    var localCount = functionState.GetLocalCount();

                    var functionSymbol = allocator.FindOrDeclare(command.Name, command.Mode);

                    return new FunctionAssignExecutor(functionSymbol, localCount, functionArguments, functionBody);

                case CommandType.AssignRender:
                    allocator.EnterScope();

                    var renderBody = Compiler.CompileCommand(command.Body, allocator);

                    allocator.LeaveScope();

                    var renderSymbol = allocator.FindOrDeclare(command.Name, command.Mode);

                    return new RenderAssignExecutor(renderSymbol, renderBody);

                case CommandType.AssignValue:
                    var expression = Compiler.CompileExpression(command.Operand, allocator);
                    var valueSymbol = allocator.FindOrDeclare(command.Name, command.Mode);

                    return new ValueAssignExecutor(valueSymbol, expression);

                case CommandType.Composite:
                    var nodes = new List<IExecutor>();

                    for (; command.Type == CommandType.Composite; command = command.Next)
                        nodes.Add(Compiler.CompileCommand(command.Body, allocator));

                    nodes.Add(Compiler.CompileCommand(command, allocator));

                    return new CompositeExecutor(nodes);

                case CommandType.Dump:
                    return new DumpExecutor(Compiler.CompileExpression(command.Operand, allocator));

                case CommandType.Echo:
                    return new EchoExecutor(Compiler.CompileExpression(command.Operand, allocator));

                case CommandType.For:
                    var forSource = Compiler.CompileExpression(command.Operand, allocator);

                    allocator.EnterScope();

                    var forKey = !string.IsNullOrEmpty(command.Key)
                        ? (int?)allocator.DeclareAsLocal(command.Key)
                        : null;
                    var forValue = allocator.DeclareAsLocal(command.Name);

                    var forBody = Compiler.CompileCommand(command.Body, allocator);
                    var forEmpty = command.Next != null ? Compiler.CompileCommand(command.Next, allocator) : null;

                    allocator.LeaveScope();

                    return new ForExecutor(forSource, forKey, forValue, forBody, forEmpty);

                case CommandType.If:
                    var ifBranches = new List<KeyValuePair<IEvaluator, IExecutor>>();

                    for (; command != null && command.Type == CommandType.If; command = command.Next)
                    {
                        var condition = Compiler.CompileExpression(command.Operand, allocator);

                        allocator.EnterScope();

                        var body = Compiler.CompileCommand(command.Body, allocator);

                        allocator.LeaveScope();

                        ifBranches.Add(new KeyValuePair<IEvaluator, IExecutor>(condition, body));
                    }

                    IExecutor ifFallback;

                    if (command != null)
                    {
                        allocator.EnterScope();

                        ifFallback = Compiler.CompileCommand(command, allocator);

                        allocator.LeaveScope();
                    }
                    else
                        ifFallback = null;

                    return new IfExecutor(ifBranches, ifFallback);

                case CommandType.Literal:
                    return new LiteralExecutor(command.Text);

                case CommandType.Return:
                    return new ReturnExecutor(Compiler.CompileExpression(command.Operand, allocator));

                case CommandType.While:
                    var whileCondition = Compiler.CompileExpression(command.Operand, allocator);

                    allocator.EnterScope();

                    var whileBody = Compiler.CompileCommand(command.Body, allocator);

                    allocator.LeaveScope();

                    return new WhileExecutor(whileCondition, whileBody);

                default:
                    return new LiteralExecutor(string.Empty);
            }
        }

        private static IEvaluator CompileExpression(Expression expression, Allocator allocator)
        {
            switch (expression.Type)
            {
                case ExpressionType.Access:
                    return new AccessEvaluator(Compiler.CompileExpression(expression.Source, allocator),
                        Compiler.CompileExpression(expression.Subscript, allocator));

                case ExpressionType.Constant:
                    return new ConstantEvaluator(expression.Value);

                case ExpressionType.Invoke:
                    var arguments = new IEvaluator[expression.Arguments.Length];

                    for (var i = 0; i < arguments.Length; ++i)
                        arguments[i] = Compiler.CompileExpression(expression.Arguments[i], allocator);

                    return new InvokeEvaluator(Compiler.CompileExpression(expression.Source, allocator), arguments);

                case ExpressionType.Map:
                    var elements = new KeyValuePair<IEvaluator, IEvaluator>[expression.Elements.Length];

                    for (var i = 0; i < elements.Length; ++i)
                    {
                        var key = Compiler.CompileExpression(expression.Elements[i].Key, allocator);
                        var value = Compiler.CompileExpression(expression.Elements[i].Value, allocator);

                        elements[i] = new KeyValuePair<IEvaluator, IEvaluator>(key, value);
                    }

                    return new MapEvaluator(elements);

                case ExpressionType.Symbol:
                    return new SymbolEvaluator(allocator.FindOrDeclare(expression.Value, StoreMode.Global));

                default:
                    return VoidEvaluator.Instance;
            }
        }
    }
}