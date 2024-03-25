using System.Collections.Generic;

namespace Cottle.Documents.Emitted
{
    internal readonly struct Frame
    {
        public readonly IReadOnlyList<Value> Arguments;
        public readonly IReadOnlyList<Value> Constants;

        public Frame(IReadOnlyList<Value> constants, IReadOnlyList<Value> arguments)
        {
            Arguments = arguments;
            Constants = constants;
        }
    }
}