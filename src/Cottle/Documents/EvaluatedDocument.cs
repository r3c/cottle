using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents
{
    internal class EvaluatedDocument : CompiledDocument<IExecutor, IExecutor>
    {
        public EvaluatedDocument(Statement statement) :
            base(new Compiler(), e => e, statement)
        {
        }

        protected override Value Execute(IExecutor executable, Frame frame, TextWriter writer)
        {
            executable.Execute(frame, writer, out var result);

            return result;
        }
    }
}