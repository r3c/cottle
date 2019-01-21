using System.IO;
using System.Text;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
    internal class LiteralNode : INode
    {
        #region Attributes

        private readonly string _text;

        #endregion

        #region Constructors

        public LiteralNode(string text)
        {
            _text = text;
        }

        #endregion

        #region Methods

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            output.Write(_text);

            result = VoidValue.Instance;

            return false;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            var builder = new StringBuilder()
                .Append(_text)
                .Replace("\\", "\\\\")
                .Replace(setting.BlockBegin, "\\" + setting.BlockBegin)
                .Replace(setting.BlockContinue, "\\" + setting.BlockContinue)
                .Replace(setting.BlockEnd, "\\" + setting.BlockEnd);

            output.Write(builder);
        }

        #endregion
    }
}