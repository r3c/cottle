using System.Collections.Generic;
using Cottle.Documents.Compiled;
using Cottle.Documents.Compiled.Compilers;
using Cottle.Documents.Evaluated.Evaluators;
using Cottle.Documents.Evaluated.Executors;
using Cottle.Documents.Evaluated.Executors.Assign;

namespace Cottle.Documents.Evaluated
{
    internal class Compiler : AbstractCompiler<IExecutor, IEvaluator>
    {
        protected override IExecutor CreateStatementAssignFunction(Symbol symbol, int localCount,
            IReadOnlyList<Symbol> arguments, IExecutor body)
        {
            return new FunctionAssignExecutor(symbol, localCount, arguments, body);
        }

        protected override IExecutor CreateStatementAssignRender(Symbol symbol, IExecutor body)
        {
            return new RenderAssignExecutor(symbol, body);
        }

        protected override IExecutor CreateStatementAssignValue(Symbol symbol, IEvaluator expression)
        {
            return new ValueAssignExecutor(symbol, expression);
        }

        protected override IExecutor CreateStatementComposite(IReadOnlyList<IExecutor> statements)
        {
            return new CompositeExecutor(statements);
        }

        protected override IExecutor CreateStatementDump(IEvaluator expression)
        {
            return new DumpExecutor(expression);
        }

        protected override IExecutor CreateStatementEcho(IEvaluator expression)
        {
            return new EchoExecutor(expression);
        }

        protected override IExecutor CreateStatementFor(IEvaluator source, Symbol? key, Symbol value, IExecutor body,
            IExecutor empty)
        {
            return new ForExecutor(source, key, value, body, empty);
        }

        protected override IExecutor CreateStatementIf(IReadOnlyList<KeyValuePair<IEvaluator, IExecutor>> branches,
            IExecutor fallback)
        {
            return new IfExecutor(branches, fallback);
        }

        protected override IExecutor CreateStatementLiteral(string text)
        {
            return new LiteralExecutor(text);
        }

        protected override IExecutor CreateStatementNone()
        {
            return new LiteralExecutor(string.Empty);
        }

        protected override IExecutor CreateStatementReturn(IEvaluator expression)
        {
            return new ReturnExecutor(expression);
        }

        protected override IExecutor CreateStatementWhile(IEvaluator condition, IExecutor body)
        {
            return new WhileExecutor(condition, body);
        }

        protected override IEvaluator CreateExpressionAccess(IEvaluator source, IEvaluator subscript)
        {
            return new AccessEvaluator(source, subscript);
        }

        protected override IEvaluator CreateExpressionConstant(Value value)
        {
            return new ConstantEvaluator(value);
        }

        protected override IEvaluator CreateExpressionInvoke(IEvaluator caller, IReadOnlyList<IEvaluator> arguments)
        {
            return new InvokeEvaluator(caller, arguments);
        }

        protected override IEvaluator CreateExpressionMap(IReadOnlyList<KeyValuePair<IEvaluator, IEvaluator>> elements)
        {
            return new MapEvaluator(elements);
        }

        protected override IEvaluator CreateExpressionSymbol(Symbol symbol)
        {
            return new SymbolEvaluator(symbol);
        }

        protected override IEvaluator CreateExpressionVoid()
        {
            return VoidEvaluator.Instance;
        }
    }
}