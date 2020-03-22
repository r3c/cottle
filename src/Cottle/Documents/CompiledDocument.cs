using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents
{
    internal abstract class CompiledDocument<TAssembly, TExecutable> : IDocument
    {
        private readonly TExecutable _executable;

        private readonly IReadOnlyList<Value> _globals;

        private readonly int _locals;

        protected CompiledDocument(ICompiler<TAssembly> compiler, Func<TAssembly, TExecutable> assemble,
            Statement statement)
        {
            var (program, globals, locals) = compiler.Compile(statement);

            _executable = assemble(program);
            _globals = globals;
            _locals = locals;
        }

        public Value Render(IContext context, TextWriter writer)
        {
            var globals = new Value[_globals.Count];

            for (var i = 0; i < _globals.Count; ++i)
                globals[i] = context[_globals[i]];

            return Execute(_executable, new Frame(globals, _locals), writer);
        }

        public string Render(IContext context)
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                Render(context, writer);

                return writer.ToString();
            }
        }
        
        protected abstract Value Execute(TExecutable executable, Frame frame, TextWriter writer);
    }
}