using System.Collections.Generic;
using Cottle.Documents.Compiled;
using Cottle.Documents.Compiled.Compilers;
using Cottle.Documents.Emitted.Generators;
using Cottle.Values;

namespace Cottle.Documents.Emitted
{
    internal class Compiler : AbstractCompiler<IGenerator, IGenerator>
    {
        protected override IGenerator CreateCommandAssignFunction(Symbol symbol, int localCount,
            IReadOnlyList<int> arguments, IGenerator body)
        {
            return new CommandAssignFunctionGenerator(symbol, localCount, arguments, body);
        }

        protected override IGenerator CreateCommandAssignRender(Symbol symbol, IGenerator body)
        {
            return new CommandAssignRenderGenerator(symbol, body);
        }

        protected override IGenerator CreateCommandAssignValue(Symbol symbol, IGenerator expression)
        {
            return new CommandAssignValueGenerator(symbol, expression);
        }

        protected override IGenerator CreateCommandComposite(IReadOnlyList<IGenerator> commands)
        {
            return new CommandCompositeGenerator(commands);
        }

        protected override IGenerator CreateCommandDump(IGenerator expression)
        {
            return new CommandDumpGenerator(expression);
        }

        protected override IGenerator CreateCommandEcho(IGenerator expression)
        {
            return new CommandEchoGenerator(expression);
        }

        protected override IGenerator CreateCommandFor(IGenerator source, int? key, int value, IGenerator body,
            IGenerator empty)
        {
            return new CommandForGenerator(source, key, value, body, empty);
        }

        protected override IGenerator CreateCommandIf(IReadOnlyList<KeyValuePair<IGenerator, IGenerator>> branches,
            IGenerator fallback)
        {
            return new CommandIfGenerator(branches, fallback);
        }

        protected override IGenerator CreateCommandLiteral(string text)
        {
            return new CommandLiteralGenerator(text);
        }

        protected override IGenerator CreateCommandNone()
        {
            return new CommandNoneGenerator();
        }

        protected override IGenerator CreateCommandReturn(IGenerator expression)
        {
            return new CommandReturnGenerator(expression);
        }

        protected override IGenerator CreateCommandWhile(IGenerator condition, IGenerator body)
        {
            return new CommandWhileGenerator(condition, body);
        }

        protected override IGenerator CreateExpressionAccess(IGenerator source, IGenerator subscript)
        {
            return new ExpressionAccessGenerator(source, subscript);
        }

        protected override IGenerator CreateExpressionConstant(Value value)
        {
            return new ExpressionConstantGenerator(value);
        }

        protected override IGenerator CreateExpressionInvoke(IGenerator caller, IReadOnlyList<IGenerator> arguments)
        {
            return new ExpressionInvokeGenerator(caller, arguments);
        }

        protected override IGenerator CreateExpressionMap(IReadOnlyList<KeyValuePair<IGenerator, IGenerator>> elements)
        {
            return new ExpressionMapGenerator(elements);
        }

        protected override IGenerator CreateExpressionSymbol(Symbol symbol)
        {
            return new ExpressionSymbolGenerator(symbol);
        }

        protected override IGenerator CreateExpressionVoid()
        {
            return new ExpressionConstantGenerator(VoidValue.Instance);
        }
    }
}