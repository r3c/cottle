using System.IO;
using Cottle.Values;

namespace Cottle.Documents
{
    /// <summary>
    /// Empty document always renders an empty result and returns a void value.
    /// </summary>
    public sealed class EmptyDocument : AbstractDocument
    {
        public static EmptyDocument Instance { get; } = new EmptyDocument();

        public override Value Render(IContext context, TextWriter writer)
        {
            return VoidValue.Instance;
        }
    }
}