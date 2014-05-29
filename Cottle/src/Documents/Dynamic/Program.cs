using System.IO;

namespace Cottle.Documents.Dynamic
{
	delegate Value Renderer (Storage storage, IScope scope, TextWriter writer); 
}
