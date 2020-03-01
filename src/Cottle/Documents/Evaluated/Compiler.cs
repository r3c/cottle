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
        protected override IExecutor CreateCommandAssignFunction(Symbol symbol, int localCount,
            IReadOnlyList<int> arguments, IExecutor body)
        {
            return new FunctionAssignExecutor(symbol, localCount, arguments, body);
        }

        protected override IExecutor CreateCommandAssignRender(Symbol symbol, IExecutor body)
        {
            return new RenderAssignExecutor(symbol, body);
        }

        protected override IExecutor CreateCommandAssignValue(Symbol symbol, IEvaluator expression)
        {
            return new ValueAssignExecutor(symbol, expression);
        }

        protected override IExecutor CreateCommandComposite(IReadOnlyList<IExecutor> commands)
        {
            return new CompositeExecutor(commands);
        }

        protected override IExecutor CreateCommandDump(IEvaluator expression)
        {
            return new DumpExecutor(expression);
        }

        protected override IExecutor CreateCommandEcho(IEvaluator expression)
        {
            return new EchoExecutor(expression);
        }

        protected override IExecutor CreateCommandFor(IEvaluator source, int? key, int value, IExecutor body, IExecutor empty)
        {
            return new ForExecutor(source, key, value, body, empty);
        }

        protected override IExecutor CreateCommandIf(IReadOnlyList<KeyValuePair<IEvaluator, IExecutor>> branches,
            IExecutor fallback)
        {
            return new IfExecutor(branches, fallback);
        }

        protected override IExecutor CreateCommandLiteral(string text)
        {
            return new LiteralExecutor(text);
        }

        protected override IExecutor CreateCommandNone()
        {
            return new LiteralExecutor(string.Empty);
        }

        protected override IExecutor CreateCommandReturn(IEvaluator expression)
        {
            return new ReturnExecutor(expression);
        }

        protected override IExecutor CreateCommandWhile(IEvaluator condition, IExecutor body)
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