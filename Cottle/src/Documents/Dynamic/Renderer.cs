using System.IO;

namespace Cottle.Documents.Dynamic
{
	delegate Value Renderer (string[] strings, Value[] values, IScope scope, TextWriter writer); 
}
