using System;
using System.Collections.Generic;
using Cottle.Documents.Simple.Evaluators;
using Cottle.Documents.Simple.Nodes;
using Cottle.Documents.Simple.Nodes.AssignNodes;

namespace Cottle.Documents.Simple
{
    internal static class Compiler
    {
        public static INode Compile(Statement statement)
        {
            return Compiler.CompileStatement(statement);
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
                    var arguments = new IEvaluator[expression.Arguments.Count];

                    for (var i = 0; i < arguments.Length; ++i)
                        arguments[i] = Compiler.CompileExpression(expression.Arguments[i]);

                    return new InvokeEvaluator(Compiler.CompileExpression(expression.Source), arguments);

                case ExpressionType.Map:
                    var elements = new KeyValuePair<IEvaluator, IEvaluator>[expression.Elements.Count];

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
                    throw new ArgumentOutOfRangeException(nameof(expression));
            }
        }

        private static INode CompileStatement(Statement statement)
        {
            switch (statement.Type)
            {
                case StatementType.AssignFunction:
                    return new FunctionAssignNode(statement.Key, statement.Arguments,
                        Compiler.CompileStatement(statement.Body), statement.Mode);

                case StatementType.AssignRender:
                    return new RenderAssignNode(statement.Key, Compiler.CompileStatement(statement.Body),
                        statement.Mode);

                case StatementType.AssignValue:
                    return new ValueAssignNode(statement.Key, Compiler.CompileExpression(statement.Operand),
                        statement.Mode);

                case StatementType.Composite:
                    var nodes = new List<INode>();

                    for (; statement.Type == StatementType.Composite; statement = statement.Next)
                        nodes.Add(Compiler.CompileStatement(statement.Body));

                    nodes.Add(Compiler.CompileStatement(statement));

                    return new CompositeNode(nodes);

                case StatementType.Dump:
                    return new DumpNode(Compiler.CompileExpression(statement.Operand));

                case StatementType.Echo:
                    return new EchoNode(Compiler.CompileExpression(statement.Operand));

                case StatementType.For:
                    return new ForNode(Compiler.CompileExpression(statement.Operand), statement.Key, statement.Value,
                        Compiler.CompileStatement(statement.Body),
                        statement.Next.Type != StatementType.None ? Compiler.CompileStatement(statement.Next) : null);

                case StatementType.If:
                    var branches = new List<KeyValuePair<IEvaluator, INode>>();

                    for (; statement.Type == StatementType.If; statement = statement.Next)
                    {
                        var condition = Compiler.CompileExpression(statement.Operand);
                        var body = Compiler.CompileStatement(statement.Body);

                        branches.Add(new KeyValuePair<IEvaluator, INode>(condition, body));
                    }

                    var fallback = statement.Type != StatementType.None ? Compiler.CompileStatement(statement) : null;

                    return new IfNode(branches, fallback);

                case StatementType.Literal:
                    return new LiteralNode(statement.Value);

                case StatementType.None:
                    return new LiteralNode(string.Empty);

                case StatementType.Return:
                    return new ReturnNode(Compiler.CompileExpression(statement.Operand));

                case StatementType.Unwrap:
                    throw new NotImplementedException(
                        $"'unwrap' is not supported with {nameof(SimpleDocument)}, use {nameof(Document)}.{nameof(Document.CreateDefault)} instead");

                case StatementType.While:
                    return new WhileNode(Compiler.CompileExpression(statement.Operand),
                        Compiler.CompileStatement(statement.Body));

                case StatementType.Wrap:
                    throw new NotImplementedException(
                        $"'wrap' is not supported with {nameof(SimpleDocument)}, use {nameof(Document)}.{nameof(Document.CreateDefault)} instead");

                default:
                    throw new ArgumentOutOfRangeException(nameof(statement));
            }
        }
    }
}