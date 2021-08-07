using System.IO;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents
{
    internal class EvaluatedDocument : CompiledDocument<IStatementExecutor, IStatementExecutor>
    {
        public EvaluatedDocument(Statement statement) :
            base(new Assembler(), e => e, statement)
        {
        }

        protected override Value Execute(IStatementExecutor executable, Value[] globals, int locals, TextWriter writer)
        {
            var frame = new Frame(globals, locals, null);

            return executable.Execute(frame, writer).GetValueOrDefault(Value.Undefined);
        }
    }
}