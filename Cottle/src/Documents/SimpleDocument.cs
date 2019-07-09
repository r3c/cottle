using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Documents.Simple;
using Cottle.Documents.Simple.Evaluators;
using Cottle.Documents.Simple.Nodes;
using Cottle.Documents.Simple.Nodes.AssignNodes;
using Cottle.Settings;
using Cottle.Stores;

namespace Cottle.Documents
{
    /// <summary>
    /// Simple document renders templates using an interpreter. If offers better garbage collection and easier debugging
    /// but average rendering performance.
    /// </summary>
    public sealed class SimpleDocument : AbstractDocument
    {
        private readonly INode _renderer;

        private readonly ISetting _setting;

        public SimpleDocument(TextReader reader, ISetting setting)
        {
            var parser = ParserFactory.BuildParser(setting);
            var root = parser.Parse(reader);

            _renderer = CompileCommand(root, setting.Trimmer);
            _setting = setting;
        }

        public SimpleDocument(TextReader reader) :
            this(reader, DefaultSetting.Instance)
        {
        }

        public SimpleDocument(string template, ISetting setting) :
            this(new StringReader(template), setting)
        {
        }

        public SimpleDocument(string template) :
            this(new StringReader(template), DefaultSetting.Instance)
        {
        }

        public override Value Render(IContext context, TextWriter writer)
        {
            _renderer.Render(new ContextStore(context), writer, out var result);

            return result;
        }

        public void Source(TextWriter writer)
        {
            _renderer.Source(_setting, writer);
        }

        public string Source()
        {
            var writer = new StringWriter(CultureInfo.InvariantCulture);

            Source(writer);

            return writer.ToString();
        }

        private INode CompileCommand(Command command, Trimmer trimmer)
        {
            switch (command.Type)
            {
                case CommandType.AssignFunction:
                    return new FunctionAssignNode(command.Name, command.Arguments,
                        CompileCommand(command.Body, trimmer), command.Mode);

                case CommandType.AssignRender:
                    return new RenderAssignNode(command.Name, CompileCommand(command.Body, trimmer), command.Mode);

                case CommandType.AssignValue:
                    return new ValueAssignNode(command.Name, CompileExpression(command.Operand), command.Mode);

                case CommandType.Composite:
                    var nodes = new List<INode>();

                    for (; command.Type == CommandType.Composite; command = command.Next)
                        nodes.Add(CompileCommand(command.Body, trimmer));

                    nodes.Add(CompileCommand(command, trimmer));

                    return new CompositeNode(nodes);

                case CommandType.Dump:
                    return new DumpNode(CompileExpression(command.Operand));

                case CommandType.Echo:
                    return new EchoNode(CompileExpression(command.Operand));

                case CommandType.For:
                    return new ForNode(CompileExpression(command.Operand), command.Key, command.Name,
                        CompileCommand(command.Body, trimmer),
                        command.Next != null ? CompileCommand(command.Next, trimmer) : null);

                case CommandType.If:
                    var branches = new List<KeyValuePair<IEvaluator, INode>>();

                    for (; command != null && command.Type == CommandType.If; command = command.Next)
                        branches.Add(new KeyValuePair<IEvaluator, INode>(CompileExpression(command.Operand),
                            CompileCommand(command.Body, trimmer)));

                    return new IfNode(branches, command != null ? CompileCommand(command, trimmer) : null);

                case CommandType.Literal:
                    return new LiteralNode(trimmer(command.Text));

                case CommandType.Return:
                    return new ReturnNode(CompileExpression(command.Operand));

                case CommandType.While:
                    return new WhileNode(CompileExpression(command.Operand),
                        CompileCommand(command.Body, trimmer));

                default:
                    return new LiteralNode(string.Empty);
            }
        }

        private IEvaluator CompileExpression(Expression expression)
        {
            switch (expression.Type)
            {
                case ExpressionType.Access:
                    return new AccessEvaluator(CompileExpression(expression.Source),
                        CompileExpression(expression.Subscript));

                case ExpressionType.Constant:
                    return new ConstantEvaluator(expression.Value);

                case ExpressionType.Invoke:
                    var arguments = new IEvaluator[expression.Arguments.Length];

                    for (var i = 0; i < arguments.Length; ++i)
                        arguments[i] = CompileExpression(expression.Arguments[i]);

                    return new InvokeEvaluator(CompileExpression(expression.Source), arguments);

                case ExpressionType.Map:
                    var elements = new KeyValuePair<IEvaluator, IEvaluator>[expression.Elements.Length];

                    for (var i = 0; i < elements.Length; ++i)
                    {
                        var key = CompileExpression(expression.Elements[i].Key);
                        var value = CompileExpression(expression.Elements[i].Value);

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