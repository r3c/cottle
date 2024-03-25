using System;

namespace Cottle.Documents.Evaluated
{
    internal readonly struct Frame
    {
        public readonly Value[] Locals;

        public Frame(int localCount)
        {
            Locals = localCount > 0 ? new Value[localCount] : Array.Empty<Value>();
        }
    }
}