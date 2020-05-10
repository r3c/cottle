using System.Collections.Generic;
using Cottle.Documents.Compiled;
using Cottle.Documents.Compiled.Compilers;
using Cottle.Documents.Emitted.ExpressionGenerators;
using Cottle.Documents.Emitted.StatementGenerators;

namespace Cottle.Documents.Emitted
{
    internal class Compiler : AbstractCompiler<IStatementGenerator, IExpressionGenerator>
    {
        protected override IExpressionGenerator CreateExpressionAccess(IExpressionGenerator source,
            IExpressionGenerator subscript)
        {
            return new AccessExpressionGenerator(source, subscript);
        }

        protected override IExpressionGenerator CreateExpressionConstant(Value value)
        {
            return new ConstantExpressionGenerator(value);
        }

        protected override IExpressionGenerator CreateExpressionInvoke(IExpressionGenerator caller,
            IReadOnlyList<IExpressionGenerator> arguments)
        {
            return new InvokeExpressionGenerator(caller, arguments);
        }

        protected override IExpressionGenerator CreateExpressionMap(
            IReadOnlyList<KeyValuePair<IExpressionGenerator, IExpressionGenerator>> elements)
        {
            return new MapExpressionGenerator(elements);
        }

        protected override IExpressionGenerator CreateExpressionSymbol(Symbol symbol)
        {
            return new SymbolExpressionGenerator(symbol);
        }

        protected override IExpressionGenerator CreateExpressionVoid()
        {
            return new ConstantExpressionGenerator(Value.Undefined);
        }

        protected override IStatementGenerator CreateStatementAssignFunction(Symbol symbol, int localCount,
            IReadOnlyList<Symbol> arguments, IStatementGenerator body)
        {
            return new AssignFunctionStatementGenerator(symbol, localCount, arguments, body);
        }

        protected override IStatementGenerator CreateStatementAssignRender(Symbol symbol, IStatementGenerator body)
        {
            return new AssignRenderStatementGenerator(symbol, body);
        }

        protected override IStatementGenerator CreateStatementAssignValue(Symbol symbol,
            IExpressionGenerator expression)
        {
            return new AssignValueStatementGenerator(symbol, expression);
        }

        protected override IStatementGenerator CreateStatementComposite(IReadOnlyList<IStatementGenerator> statements)
        {
            return new CompositeStatementGenerator(statements);
        }

        protected override IStatementGenerator CreateStatementDump(IExpressionGenerator expression)
        {
            return new DumpStatementGenerator(expression);
        }

        protected override IStatementGenerator CreateStatementEcho(IExpressionGenerator expression)
        {
            return new EchoStatementGenerator(expression);
        }

        protected override IStatementGenerator CreateStatementFor(IExpressionGenerator source, Symbol? key,
            Symbol value, IStatementGenerator body, IStatementGenerator empty)
        {
            return new ForStatementGenerator(source, key, value, body, empty);
        }

        protected override IStatementGenerator CreateStatementIf(
            IReadOnlyList<KeyValuePair<IExpressionGenerator, IStatementGenerator>> branches,
            IStatementGenerator fallback)
        {
            return new IfStatementGenerator(branches, fallback);
        }

        protected override IStatementGenerator CreateStatementLiteral(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new NoneStatementGenerator();

            return new LiteralStatementGenerator(text);
        }

        protected override IStatementGenerator CreateStatementNone()
        {
            return new NoneStatementGenerator();
        }

        protected override IStatementGenerator CreateStatementReturn(IExpressionGenerator expression)
        {
            return new ReturnStatementGenerator(expression);
        }

        protected override IStatementGenerator CreateStatementUnwrap(IStatementGenerator body)
        {
            return new UnwrapStatementGenerator(body);
        }

        protected override IStatementGenerator CreateStatementWhile(IExpressionGenerator condition,
            IStatementGenerator body)
        {
            return new WhileStatementGenerator(condition, body);
        }

        protected override IStatementGenerator CreateStatementWrap(IExpressionGenerator modifier,
            IStatementGenerator body)
        {
            return new WrapStatementGenerator(modifier, body);
        }
    }
}