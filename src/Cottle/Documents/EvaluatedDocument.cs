using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents
{
    internal class EvaluatedDocument : CompiledDocument<IStatementExecutor, IStatementExecutor>
    {
        public EvaluatedDocument(Statement statement) :
            base(new Assembler(), e => e, statement)
        {
        }

        protected override Value Execute(IStatementExecutor executable, Frame frame, TextWriter writer)
        {
            return executable.Execute(frame, writer).GetValueOrDefault(Value.Undefined);
        }
    }
}