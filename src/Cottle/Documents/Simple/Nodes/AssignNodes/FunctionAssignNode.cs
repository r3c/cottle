using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cottle.Documents.Simple.Nodes.AssignNodes
{
    internal class FunctionAssignNode : AssignNode, IFunction
    {
        public bool IsPure => false;

        private readonly IReadOnlyList<string> _arguments;

        private readonly INode _body;

        public FunctionAssignNode(string name, IEnumerable<string> arguments, INode body, StoreMode mode) :
            base(name, mode)
        {
            _arguments = arguments.ToList();
            _body = body;
        }

        public int CompareTo(IFunction? other)
        {
            return object.ReferenceEquals(this, other) ? 0 : 1;
        }

        public bool Equals(IFunction? other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is IFunction other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    (_body.GetHashCode() & (int)0xFFFFFF00) |
                    (base.GetHashCode() & 0x000000FF);
            }
        }

        public Value Invoke(object? state, IReadOnlyList<Value> arguments, TextWriter output)
        {
            if (state is not IStore store)
            {
                throw new InvalidOperationException(
                    $"Invalid function invoke, you seem to have injected a function declared in a {nameof(SimpleDocument)} from another type of document.");
            }

            store.Enter();

            for (var i = 0; i < _arguments.Count; ++i)
                store.Set(_arguments[i], i < arguments.Count ? arguments[i] : Value.Undefined, StoreMode.Local);

            _body.Render(store, output, out var result);

            store.Leave();

            return result;
        }

        protected override Value Evaluate(IStore store, TextWriter output)
        {
            return Value.FromFunction(this);
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
    }
}