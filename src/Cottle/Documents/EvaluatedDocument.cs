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

        protected override Value Execute(IStatementExecutor executable, Runtime runtime, int locals,
            TextWriter writer)
        {
            var frame = new Frame(locals, null);

            return executable.Execute(runtime, frame, writer).GetValueOrDefault(Value.Undefined);
        }
    }
}