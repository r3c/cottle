using System;
using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Emitted;

namespace Cottle.Documents
{
    internal class EmittedDocument : CompiledDocument<IStatementGenerator, Program>
    {
        public EmittedDocument(RenderConfiguration configuration, Statement statement) :
            base(new Assembler(), generator => Program.Create(generator, Array.Empty<Symbol>()), configuration,
                statement)
        {
        }

        protected override Value Execute(Program program, Runtime runtime, int locals, TextWriter writer)
        {
            return program.Execute(runtime, Array.Empty<Value>(), writer);
        }
    }
}