using System.IO;

namespace Cottle.Documents.Dynamic
{
	delegate Value Renderer (DynamicDocument document, IScope scope, TextWriter writer); 
}
