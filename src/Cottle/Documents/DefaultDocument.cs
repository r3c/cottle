using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Documents.Default;

namespace Cottle.Documents
{
    internal class DefaultDocument : IDocument
    {
        private readonly IExecutor _executor;

        private readonly IReadOnlyList<Value> _globals;

        private readonly int _localCount;

        public DefaultDocument(Command command)
        {
            var (executor, globals, localCount) = Compiler.Compile(command);

            _executor = executor;
            _globals = globals;
            _localCount = localCount;
        }

        public Value Render(IContext context, TextWriter writer)
        {
            var globals = new Value[_globals.Count];

            for (var i = 0; i < _globals.Count; ++i)
                globals[i] = context[_globals[i]];

            _executor.Execute(new Frame(globals, _localCount), writer, out var result);

            return result;
        }

        public string Render(IContext context)
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                Render(context, writer);

                return writer.ToString();
            }
        }
    }
}