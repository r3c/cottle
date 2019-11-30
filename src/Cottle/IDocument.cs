using System.IO;

namespace Cottle
{
    public interface IDocument
    {
        Value Render(IContext context, TextWriter writer);

        string Render(IContext context);
    }
}