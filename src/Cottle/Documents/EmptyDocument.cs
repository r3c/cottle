using System.IO;

namespace Cottle.Documents
{
    /// <summary>
    /// Empty document always renders an empty result and returns an undefined value.
    /// </summary>
    public sealed class EmptyDocument : AbstractDocument
    {
        public static EmptyDocument Instance { get; } = new EmptyDocument();

        public override Value Render(IContext context, TextWriter writer)
        {
            return Value.Undefined;
        }
    }
}