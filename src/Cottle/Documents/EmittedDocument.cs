using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Emitted;
using Cottle.Values;

namespace Cottle.Documents
{
    internal class EmittedDocument : CompiledDocument<IStatementGenerator, Program>
    {
        public EmittedDocument(Statement statement) :
            base(new Compiler(), Program.Create, statement)
        {
        }

        protected override Value Execute(Program program, Frame frame, TextWriter writer)
        {
            return program.Executable(program.Constants, frame, writer, out var result) ? result : VoidValue.Instance;
        }
    }
}