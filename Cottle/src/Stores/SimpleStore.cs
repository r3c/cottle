using System.Collections.Generic;

namespace Cottle.Stores
{
    public sealed class SimpleStore : AbstractStore
    {
        private readonly Stack<HashSet<Value>> frames;

        private readonly Dictionary<Value, Stack<Value>> stacks;
        private int depth;

        public SimpleStore()
        {
            depth = 0;
            frames = new Stack<HashSet<Value>>();
            stacks = new Dictionary<Value, Stack<Value>>();
        }

        public override void Enter()
        {
            // Stack frames are lazily built (see "Set" method), we just keep trace of current stack frame depth here
            ++depth;
        }

        public override bool Leave()
        {
            if (depth < 1)
                return false;

            // Early exit if no stack frames were allocated at parent depth level
            if (--depth >= frames.Count)
                return true;

            // Delete all current stack frame symbols
            foreach (var name in frames.Pop())
            {
                if (!stacks.TryGetValue(name, out var stack))
                    continue;

                if (stack.Count < 2)
                    stacks.Remove(name);
                else
                    stack.Pop();
            }

            return true;
        }

        public override void Set(Value symbol, Value value, StoreMode mode)
        {
            if (!stacks.TryGetValue(symbol, out var stack))
            {
                stack = new Stack<Value>();

                stacks[symbol] = stack;
            }

            switch (mode)
            {
                case StoreMode.Global:
                    if (stack.Count > 0)
                        stack.Pop();

                    break;

                case StoreMode.Local:
                    // Lazily create stack frames to current depth
                    while (depth > frames.Count)
                        frames.Push(new HashSet<Value>());

                    // Erase symbol if it previously existed in current stack frame
                    if (frames.Count > 0 && !frames.Peek().Add(symbol))
                        stack.Pop();

                    break;
            }

            stack.Push(value);
        }

        public override bool TryGet(Value symbol, out Value value)
        {
            if (stacks.TryGetValue(symbol, out var stack) && stack.Count > 0)
            {
                value = stack.Peek();

                return true;
            }

            value = null;

            return false;
        }
    }
}