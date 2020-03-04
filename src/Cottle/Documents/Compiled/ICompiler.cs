using System.Collections.Generic;

namespace Cottle.Documents.Compiled
{
    internal interface ICompiler<TAssembly>
    {
        (TAssembly, IReadOnlyList<Value>, int) Compile(Command command);
    }
}