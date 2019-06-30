using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes.AssignNodes
{
    internal class FunctionAssignNode : AssignNode, IFunction
    {
        public FunctionAssignNode(string name, IEnumerable<string> arguments, INode body, StoreMode mode) :
            base(name, mode)
        {
            this.arguments = arguments.ToArray();
            this.body = body;
        }

        private readonly string[] arguments;

        private readonly INode body;

        public int CompareTo(IFunction other)
        {
            return object.ReferenceEquals(this, other) ? 0 : 1;
        }

        public bool Equals(IFunction other)
        {
            return this.CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is IFunction other && this.Equals(other);
        }

        public Value Execute(IReadOnlyList<Value> arguments, IStore store, TextWriter output)
        {
            store.Enter();

            for (var i = 0; i < this.arguments.Length; ++i)
                store.Set(this.arguments[i], i < arguments.Count ? arguments[i] : VoidValue.Instance, StoreMode.Local);

            this.body.Render(store, output, out var result);

            store.Leave();

            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    (this.body.GetHashCode() & (int)0xFFFFFF00) |
                    (base.GetHashCode() & 0x000000FF);
            }
        }

        protected override Value Evaluate(IStore store, TextWriter output)
        {
            return new FunctionValue(this);
        }

        protected override void SourceSymbol(string name, TextWriter output)
        {
            var comma = false;

            output.Write(name);
            output.Write('(');

            foreach (var argument in this.arguments)
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

            this.body.Source(setting, output);
        }
    }
}