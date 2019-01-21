using System.Collections.Generic;
using System.IO;

namespace Cottle.Documents.Dynamic
{
    internal delegate Value Renderer(Storage storage, IList<Value> arguments, IStore store, TextWriter output);
}