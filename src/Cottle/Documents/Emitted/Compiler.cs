using System.Collections.Generic;
using Cottle.Documents.Compiled;
using Cottle.Documents.Compiled.Compilers;
using Cottle.Documents.Emitted.CommandGenerators;
using Cottle.Documents.Emitted.ExpressionGenerators;
using Cottle.Values;

namespace Cottle.Documents.Emitted
{
    internal class Compiler : AbstractCompiler<ICommandGenerator, IExpressionGenerator>
    {
        protected override ICommandGenerator CreateCommandAssignFunction(Symbol symbol, int localCount,
            IReadOnlyList<int> arguments, ICommandGenerator body)
        {
            return new AssignFunctionCommandGenerator(symbol, localCount, arguments, body);
        }

        protected override ICommandGenerator CreateCommandAssignRender(Symbol symbol, ICommandGenerator body)
        {
            return new AssignRenderCommandGenerator(symbol, body);
        }

        protected override ICommandGenerator CreateCommandAssignValue(Symbol symbol, IExpressionGenerator expression)
        {
            return new AssignValueCommandGenerator(symbol, expression);
        }

        protected override ICommandGenerator CreateCommandComposite(IReadOnlyList<ICommandGenerator> commands)
        {
            return new CompositeCommandGenerator(commands);
        }

        protected override ICommandGenerator CreateCommandDump(IExpressionGenerator expression)
        {
            return new DumpCommandGenerator(expression);
        }

        protected override ICommandGenerator CreateCommandEcho(IExpressionGenerator expression)
        {
            return new EchoCommandGenerator(expression);
        }

        protected override ICommandGenerator CreateCommandFor(IExpressionGenerator source, int? key, int value,
            ICommandGenerator body, ICommandGenerator empty)
        {
            return new ForCommandGenerator(source, key, value, body, empty);
        }

        protected override ICommandGenerator CreateCommandIf(IReadOnlyList<KeyValuePair<IExpressionGenerator, ICommandGenerator>> branches,
            ICommandGenerator fallback)
        {
            return new IfCommandGenerator(branches, fallback);
        }

        protected override ICommandGenerator CreateCommandLiteral(string text)
        {
            return new LiteralCommandGenerator(text);
        }

        protected override ICommandGenerator CreateCommandNone()
        {
            return new NoneCommandGenerator();
        }

        protected override ICommandGenerator CreateCommandReturn(IExpressionGenerator expression)
        {
            return new ReturnCommandGenerator(expression);
        }

        protected override ICommandGenerator CreateCommandWhile(IExpressionGenerator condition, ICommandGenerator body)
        {
            return new WhileCommandGenerator(condition, body);
        }

        protected override IExpressionGenerator CreateExpressionAccess(IExpressionGenerator source, IExpressionGenerator subscript)
        {
            return new AccessExpressionGenerator(source, subscript);
        }

        protected override IExpressionGenerator CreateExpressionConstant(Value value)
        {
            return new ConstantExpressionGenerator(value);
        }

        protected override IExpressionGenerator CreateExpressionInvoke(IExpressionGenerator caller, IReadOnlyList<IExpressionGenerator> arguments)
        {
            return new InvokeExpressionGenerator(caller, arguments);
        }

        protected override IExpressionGenerator CreateExpressionMap(IReadOnlyList<KeyValuePair<IExpressionGenerator, IExpressionGenerator>> elements)
        {
            return new MapExpressionGenerator(elements);
        }

        protected override IExpressionGenerator CreateExpressionSymbol(Symbol symbol)
        {
            return new SymbolExpressionGenerator(symbol);
        }

        protected override IExpressionGenerator CreateExpressionVoid()
        {
            return new ConstantExpressionGenerator(VoidValue.Instance);
        }
    }
}