using System;
using System.Collections.Generic;

namespace Cottle.Documents.Compiled
{
    internal readonly struct Frame
    {
        public readonly Value[] Globals;
        public readonly Value[] Locals;

        public Frame(Value[] globals, int localCount)
        {
            Globals = globals;
            Locals = localCount > 0 ? new Value[localCount] : null;
        }

        public Frame CreateForFunction(IReadOnlyList<int> indices, IReadOnlyList<Value> values, int localCount)
        {
            var functionArguments = Math.Min(indices.Count, values.Count);
            var functionFrame = new Frame(Globals, localCount);

            for (var i = 0; i < functionArguments; ++i)
                functionFrame.Locals[indices[i]] = values[i];

            for (var i = values.Count; i < indices.Count; ++i)
                functionFrame.Locals[indices[i]] = Value.Undefined;

            return functionFrame;
        }
    }
}