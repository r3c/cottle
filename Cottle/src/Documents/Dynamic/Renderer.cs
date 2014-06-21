using System.Collections.Generic;
using System.IO;

namespace Cottle.Documents.Dynamic
{
	delegate Value Renderer (Storage storage, IList<Value> arguments, IStore store, TextWriter output); 
}
