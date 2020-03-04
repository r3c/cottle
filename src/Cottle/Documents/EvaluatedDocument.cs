using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents
{
    internal class EvaluatedDocument : CompiledDocument<IExecutor, IExecutor>
    {
        public EvaluatedDocument(Command command) :
            base(new Compiler(), e => e, command)
        {
        }

        protected override Value Execute(IExecutor executable, Frame frame, TextWriter writer)
        {
            executable.Execute(frame, writer, out var result);

            return result;
        }
    }
}