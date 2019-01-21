using System.IO;
using Cottle.Values;

namespace Cottle.Documents
{
    /// <summary>
    ///     Empty document always renders an empty result and returns a void value.
    /// </summary>
    public sealed class EmptyDocument : AbstractDocument
    {
        #region Attributes

        #endregion

        #region Properties

        public static EmptyDocument Instance { get; } = new EmptyDocument();

        #endregion

        #region Methods

        public override Value Render(IStore store, TextWriter writer)
        {
            return VoidValue.Instance;
        }

        #endregion
    }
}