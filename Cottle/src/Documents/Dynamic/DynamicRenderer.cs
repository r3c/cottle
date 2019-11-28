using System.Collections.Generic;
using System.IO;

namespace Cottle.Documents.Dynamic
{
    internal delegate Value DynamicRenderer(DynamicStorage storage, IStore store, IReadOnlyList<Value> arguments,
        TextWriter output);
}