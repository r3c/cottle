using System.Collections.Generic;

namespace Cottle.Documents.Compiled
{
    internal interface IAssembler<TAssembly>
    {
        (TAssembly, IReadOnlyList<Value>, int) Assemble(Statement statement);
    }
}