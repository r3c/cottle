using System.IO;

namespace Cottle.Documents.Simple.Nodes
{
    internal class ForNode : INode
    {
        public ForNode(IEvaluator from, string key, string value, INode body, INode? empty)
        {
            _body = body;
            _empty = empty;
            _from = from;
            _key = key;
            _value = value;
        }

        private readonly INode _body;

        private readonly INode? _empty;

        private readonly IEvaluator _from;

        private readonly string _key;

        private readonly string _value;

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            var fields = _from.Evaluate(store, output).Fields;

            if (fields.Count > 0)
            {
                foreach (var pair in fields)
                {
                    store.Enter();

                    if (!string.IsNullOrEmpty(_key))
                        store.Set(_key, pair.Key, StoreMode.Local);

                    store.Set(_value, pair.Value, StoreMode.Local);

                    if (_body.Render(store, output, out result))
                    {
                        store.Leave();

                        return true;
                    }

                    store.Leave();
                }
            }
            else if (_empty is not null)
            {
                store.Enter();

                if (_empty.Render(store, output, out result))
                {
                    store.Leave();

                    return true;
                }

                store.Leave();
            }

            result = Value.Undefined;

            return false;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            output.Write(setting.BlockBegin);
            output.Write("for ");

            if (!string.IsNullOrEmpty(_key))
            {
                output.Write(_key);
                output.Write(", ");
            }

            output.Write(_value);
            output.Write(" in ");
            output.Write(_from);
            output.Write(":");

            _body.Source(setting, output);

            if (_empty is not null)
            {
                output.Write(setting.BlockContinue);
                output.Write("empty:");

                _empty.Source(setting, output);
            }

            output.Write(setting.BlockEnd);
        }
    }
}