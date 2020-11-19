using System.Collections.Generic;
using Cottle.Documents.Compiled;
using Cottle.Documents.Compiled.Assemblers;
using Cottle.Documents.Evaluated.ExpressionExecutors;
using Cottle.Documents.Evaluated.StatementExecutors;
using Cottle.Documents.Evaluated.StatementExecutors.Assign;

namespace Cottle.Documents.Evaluated
{
    internal class Assembler : AbstractAssembler<IStatementExecutor, IExpressionExecutor>
    {
        protected override IStatementExecutor CreateStatementAssignFunction(Symbol symbol, int localCount,
            IReadOnlyList<Symbol> arguments, IStatementExecutor body)
        {
            return new FunctionAssignStatementExecutor(symbol, localCount, arguments, body);
        }

        protected override IStatementExecutor CreateStatementAssignRender(Symbol symbol, IStatementExecutor body)
        {
            return new RenderAssignStatementExecutor(symbol, body);
        }

        protected override IStatementExecutor CreateStatementAssignValue(Symbol symbol, IExpressionExecutor expression)
        {
            return new ValueAssignStatementExecutor(symbol, expression);
        }

        protected override IStatementExecutor CreateStatementComposite(IReadOnlyList<IStatementExecutor> statements)
        {
            return new CompositeStatementExecutor(statements);
        }

        protected override IStatementExecutor CreateStatementDump(IExpressionExecutor expression)
        {
            return new DumpStatementExecutor(expression);
        }

        protected override IStatementExecutor CreateStatementEcho(IExpressionExecutor expression)
        {
            return new EchoStatementExecutor(expression);
        }

        protected override IStatementExecutor CreateStatementFor(IExpressionExecutor source, Symbol? key, Symbol value,
            IStatementExecutor body,
            IStatementExecutor empty)
        {
            return new ForStatementExecutor(source, key, value, body, empty);
        }

        protected override IStatementExecutor CreateStatementIf(
            IReadOnlyList<KeyValuePair<IExpressionExecutor, IStatementExecutor>> branches,
            IStatementExecutor fallback)
        {
            return new IfStatementExecutor(branches, fallback);
        }

        protected override IStatementExecutor CreateStatementLiteral(string text)
        {
            return new LiteralStatementExecutor(text);
        }

        protected override IStatementExecutor CreateStatementNone()
        {
            return new LiteralStatementExecutor(string.Empty);
        }

        protected override IStatementExecutor CreateStatementReturn(IExpressionExecutor expression)
        {
            return new ReturnStatementExecutor(expression);
        }

        protected override IStatementExecutor CreateStatementUnwrap(IStatementExecutor body)
        {
            return new UnwrapStatementExecutor(body);
        }

        protected override IStatementExecutor CreateStatementWhile(IExpressionExecutor condition,
            IStatementExecutor body)
        {
            return new WhileStatementExecutor(condition, body);
        }

        protected override IStatementExecutor CreateStatementWrap(IExpressionExecutor modifier, IStatementExecutor body)
        {
            return new WrapStatementExecutor(modifier, body);
        }

        protected override IExpressionExecutor CreateExpressionAccess(IExpressionExecutor source,
            IExpressionExecutor subscript)
        {
            return new AccessExpressionExecutor(source, subscript);
        }

        protected override IExpressionExecutor CreateExpressionConstant(Value value)
        {
            return new ConstantExpressionExecutor(value);
        }

        protected override IExpressionExecutor CreateExpressionInvoke(IExpressionExecutor caller,
            IReadOnlyList<IExpressionExecutor> arguments)
        {
            return new InvokeExpressionExecutor(caller, arguments);
        }

        protected override IExpressionExecutor CreateExpressionMap(
            IReadOnlyList<KeyValuePair<IExpressionExecutor, IExpressionExecutor>> elements)
        {
            return new MapExpressionExecutor(elements);
        }

        protected override IExpressionExecutor CreateExpressionSymbol(Symbol symbol)
        {
            return new SymbolExpressionExecutor(symbol);
        }

        protected override IExpressionExecutor CreateExpressionVoid()
        {
            return VoidExpressionExecutor.Instance;
        }
    }
}