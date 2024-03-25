using System.IO;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents
{
    internal class EvaluatedDocument : CompiledDocument<IStatementExecutor, IStatementExecutor>
    {
        public EvaluatedDocument(RenderConfiguration configuration, Statement statement) :
            base(new Assembler(), e => e, configuration, statement)
        {
        }

        protected override Value Execute(IStatementExecutor executable, Runtime runtime, int locals,
            TextWriter writer)
        {
            return executable.Execute(runtime, new Frame(locals), writer).GetValueOrDefault(Value.Undefined);
        }
    }
}