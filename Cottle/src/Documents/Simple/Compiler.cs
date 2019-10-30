using System.Collections.Generic;
using Cottle.Documents.Simple.Evaluators;
using Cottle.Documents.Simple.Nodes;
using Cottle.Documents.Simple.Nodes.AssignNodes;

namespace Cottle.Documents.Simple
{
    internal static class Compiler
    {
        public static INode Compile(Command command)
        {
            return Compiler.CompileCommand(command);
        }

        private static INode CompileCommand(Command command)
        {
            switch (command.Type)
            {
                case CommandType.AssignFunction:
                    return new FunctionAssignNode(command.Name, command.Arguments,
                        Compiler.CompileCommand(command.Body), command.Mode);

                case CommandType.AssignRender:
                    return new RenderAssignNode(command.Name, Compiler.CompileCommand(command.Body), command.Mode);

                case CommandType.AssignValue:
                    return new ValueAssignNode(command.Name, Compiler.CompileExpression(command.Operand), command.Mode);

                case CommandType.Composite:
                    var nodes = new List<INode>();

                    for (; command.Type == CommandType.Composite; command = command.Next)
                        nodes.Add(Compiler.CompileCommand(command.Body));

                    nodes.Add(Compiler.CompileCommand(command));

                    return new CompositeNode(nodes);

                case CommandType.Dump:
                    return new DumpNode(Compiler.CompileExpression(command.Operand));

                case CommandType.Echo:
                    return new EchoNode(Compiler.CompileExpression(command.Operand));

                case CommandType.For:
                    return new ForNode(Compiler.CompileExpression(command.Operand), command.Key, command.Name,
                        Compiler.CompileCommand(command.Body),
                        command.Next != null ? Compiler.CompileCommand(command.Next) : null);

                case CommandType.If:
                    var branches = new List<KeyValuePair<IEvaluator, INode>>();

                    for (; command != null && command.Type == CommandType.If; command = command.Next)
                    {
                        var condition = Compiler.CompileExpression(command.Operand);
                        var body = Compiler.CompileCommand(command.Body);

                        branches.Add(new KeyValuePair<IEvaluator, INode>(condition, body));
                    }

                    return new IfNode(branches, command != null ? Compiler.CompileCommand(command) : null);

                case CommandType.Literal:
                    return new LiteralNode(command.Text);

                case CommandType.Return:
                    return new ReturnNode(Compiler.CompileExpression(command.Operand));

                case CommandType.While:
                    return new WhileNode(Compiler.CompileExpression(command.Operand),
                        Compiler.CompileCommand(command.Body));

                default:
                    return new LiteralNode(string.Empty);
            }
        }

        private static IEvaluator CompileExpression(Expression expression)
        {
            switch (expression.Type)
            {
                case ExpressionType.Access:
                    return new AccessEvaluator(Compiler.CompileExpression(expression.Source),
                        Compiler.CompileExpression(expression.Subscript));

                case ExpressionType.Constant:
                    return new ConstantEvaluator(expression.Value);

                case ExpressionType.Invoke:
                    var arguments = new IEvaluator[expression.Arguments.Length];

                    for (var i = 0; i < arguments.Length; ++i)
                        arguments[i] = Compiler.CompileExpression(expression.Arguments[i]);

                    return new InvokeEvaluator(Compiler.CompileExpression(expression.Source), arguments);

                case ExpressionType.Map:
                    var elements = new KeyValuePair<IEvaluator, IEvaluator>[expression.Elements.Length];

                    for (var i = 0; i < elements.Length; ++i)
                    {
                        var key = Compiler.CompileExpression(expression.Elements[i].Key);
                        var value = Compiler.CompileExpression(expression.Elements[i].Value);

                        elements[i] = new KeyValuePair<IEvaluator, IEvaluator>(key, value);
                    }

                    return new MapEvaluator(elements);

                case ExpressionType.Symbol:
                    return new SymbolEvaluator(expression.Value);

                default:
                    return VoidEvaluator.Instance;
            }
        }
    }
}