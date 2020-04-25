using System.IO;
using System.Text;

namespace Cottle.Documents.Simple.Nodes
{
    internal class LiteralNode : INode
    {
        private readonly string _text;

        public LiteralNode(string text)
        {
            _text = text;
        }

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            output.Write(_text);

            result = Value.Undefined;

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
    }
}