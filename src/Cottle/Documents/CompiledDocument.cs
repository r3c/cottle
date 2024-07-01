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
        private readonly IReadOnlyList<Value> _globalKeys;
        private readonly int _localCount;

        protected CompiledDocument(IAssembler<TAssembly> assembler, Func<TAssembly, TExecutable> compile,
            RenderConfiguration configuration, Statement statement)
        {
            var (assembly, globals, locals) = assembler.Assemble(statement);

            _configuration = configuration;
            _executable = compile(assembly);
            _globalKeys = globals;
            _localCount = locals;
        }

        public Value Render(IContext context, TextWriter writer)
        {
            var globalValues = new Value[_globalKeys.Count];

            for (var i = 0; i < _globalKeys.Count; ++i)
                globalValues[i] = context[_globalKeys[i]];

            var runtime = new Runtime(_globalKeys, globalValues, _configuration.NbCycleMax);

            return Execute(_executable, runtime, _localCount, writer);
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