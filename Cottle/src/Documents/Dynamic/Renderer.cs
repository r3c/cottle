using System.Collections.Generic;
using System.IO;

namespace Cottle.Documents.Dynamic
{
	delegate Value Renderer (Storage storage, IList<Value> arguments, IScope scope, TextWriter output); 
}
