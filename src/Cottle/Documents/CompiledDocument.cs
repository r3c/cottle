using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents
{
    internal abstract class CompiledDocument<TAssembly, TExecutable> : IDocument
    {
        private readonly RenderConfiguration _configuration;
        private readonly TExecutable _executable;
        private readonly IReadOnlyList<Value> _globals;
        private readonly int _locals;

        protected CompiledDocument(IAssembler<TAssembly> assembler, Func<TAssembly, TExecutable> compile,
            RenderConfiguration configuration, Statement statement)
        {
            var (assembly, globals, locals) = assembler.Assemble(statement);

            _configuration = configuration;
            _executable = compile(assembly);
            _globals = globals;
            _locals = locals;
        }

        public Value Render(IContext context, TextWriter writer)
        {
            var globals = new Value[_globals.Count];

            for (var i = 0; i < _globals.Count; ++i)
                globals[i] = context[_globals[i]];

            var runtime = new Runtime(globals, _configuration.NbCycleMax);

            return Execute(_executable, runtime, _locals, writer);
        }

        public string Render(IContext context)
        {
            using var writer = new StringWriter(CultureInfo.InvariantCulture);

            Render(context, writer);

            return writer.ToString();
        }

        protected abstract Value Execute(TExecutable executable, Runtime runtime, int locals, TextWriter writer);
    }
}