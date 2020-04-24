using System;
using System.Collections.Generic;

namespace Cottle.Documents.Compiled
{
    internal readonly struct Frame
    {
        public static Func<Frame, Value> CreateGetter(Symbol symbol)
        {
            var index = symbol.Index;

            switch (symbol.Mode)
            {
                case StoreMode.Global:
                    return frame => frame.Globals[index];

                case StoreMode.Local:
                    return frame => frame.Locals[index];

                default:
                    throw new InvalidOperationException();
            }
        }

        public static Action<Frame, Value> CreateSetter(Symbol symbol)
        {
            var index = symbol.Index;

            switch (symbol.Mode)
            {
                case StoreMode.Global:
                    return (frame, value) => frame.Globals[index] = value;

                case StoreMode.Local:
                    return (frame, value) => frame.Locals[index] = value;

                default:
                    throw new InvalidOperationException();
            }
        }

        public readonly Value[] Globals;
        public readonly Value[] Locals;

        public Frame(Value[] globals, int localCount)
        {
            Globals = globals;
            Locals = localCount > 0 ? new Value[localCount] : null;
        }

        public Frame CreateForFunction(IReadOnlyList<Symbol> arguments, IReadOnlyList<Value> values, int localCount)
        {
            var functionArguments = Math.Min(arguments.Count, values.Count);
            var functionFrame = new Frame(Globals, localCount);

            // Note: we assume all function arguments are local symbols here to avoid re-testing their mode
            for (var i = 0; i < functionArguments; ++i)
                functionFrame.Locals[arguments[i].Index] = values[i];

            for (var i = values.Count; i < arguments.Count; ++i)
                functionFrame.Locals[arguments[i].Index] = Value.Undefined;

            return functionFrame;
        }
    }
}