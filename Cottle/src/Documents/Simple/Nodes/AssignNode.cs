using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
    internal abstract class AssignNode : INode
    {
        protected AssignNode(string name, StoreMode mode)
        {
            _mode = mode;
            _name = name;
        }

        private readonly StoreMode _mode;

        private readonly string _name;

        protected abstract Value Evaluate(IStore store, TextWriter output);

        protected abstract void SourceSymbol(string name, TextWriter output);

        protected abstract void SourceValue(ISetting setting, TextWriter output);

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    (_mode.GetHashCode() & (int)0xFFFF0000) |
                    (_name.GetHashCode() & 0x0000FFFF);
            }
        }

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            store.Set(_name, Evaluate(store, output), _mode);

            result = VoidValue.Instance;

            return false;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            string keyword;
            string link;

            switch (_mode)
            {
                case StoreMode.Local:
                    keyword = "declare";
                    link = "as";

                    break;

                default:
                    keyword = "set";
                    link = "to";

                    break;
            }

            output.Write(setting.BlockBegin);
            output.Write(keyword);
            output.Write(' ');

            SourceSymbol(_name, output);

            output.Write(' ');
            output.Write(link);

            SourceValue(setting, output);

            output.Write(setting.BlockEnd);
        }

        public override string ToString()
        {
            return _name;
        }
    }
}