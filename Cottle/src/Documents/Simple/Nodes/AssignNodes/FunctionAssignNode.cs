using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes.AssignNodes
{
    internal class FunctionAssignNode : AssignNode, IFunction
    {
        #region Constructors

        public FunctionAssignNode(string name, IEnumerable<string> arguments, INode body, StoreMode mode) :
            base(name, mode)
        {
            _arguments = arguments.ToArray();
            _body = body;
        }

        #endregion

        #region Attributes

        private readonly string[] _arguments;

        private readonly INode _body;

        #endregion

        #region Methods / Public

        public int CompareTo(IFunction other)
        {
            return ReferenceEquals(this, other) ? 0 : 1;
        }

        public bool Equals(IFunction other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is IFunction other && Equals(other);
        }

        public Value Execute(IList<Value> arguments, IStore store, TextWriter output)
        {
            store.Enter();

            for (var i = 0; i < _arguments.Length; ++i)
                store.Set(_arguments[i], i < arguments.Count ? arguments[i] : VoidValue.Instance, StoreMode.Local);

            _body.Render(store, output, out var result);

            store.Leave();

            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    (_body.GetHashCode() & (int) 0xFFFFFF00) |
                    (base.GetHashCode() & 0x000000FF);
            }
        }

        #endregion

        #region Methods / Protected

        protected override Value Evaluate(IStore store, TextWriter output)
        {
            return new FunctionValue(this);
        }

        protected override void SourceSymbol(string name, TextWriter output)
        {
            var comma = false;

            output.Write(name);
            output.Write('(');

            foreach (var argument in _arguments)
            {
                if (comma)
                    output.Write(", ");
                else
                    comma = true;

                output.Write(argument);
            }

            output.Write(' ');
        }

        protected override void SourceValue(ISetting setting, TextWriter output)
        {
            output.Write(':');

            _body.Source(setting, output);
        }

        #endregion
    }
}