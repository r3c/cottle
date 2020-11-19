using System;
using System.Collections.Generic;

namespace Cottle.Stores
{
    [Obsolete("Use `Context.CreateCustom` method to get a `IContext` instance and pass it at document rendering")]
    public sealed class SimpleStore : AbstractStore
    {
        private readonly Stack<HashSet<Value>> _frames;
        private readonly Dictionary<Value, Stack<Value>> _stacks;
        private int _depth;

        public SimpleStore()
        {
            _depth = 0;
            _frames = new Stack<HashSet<Value>>();
            _stacks = new Dictionary<Value, Stack<Value>>();
        }

        public override void Enter()
        {
            // Stack frames are lazily built (see "Set" method), we just keep trace of current stack frame depth here
            ++_depth;
        }

        public override bool Leave()
        {
            if (_depth < 1)
                return false;

            // Early exit if no stack frames were allocated at parent depth level
            if (--_depth >= _frames.Count)
                return true;

            // Delete all current stack frame symbols
            foreach (var name in _frames.Pop())
            {
                if (!_stacks.TryGetValue(name, out var stack))
                    continue;

                if (stack.Count < 2)
                    _stacks.Remove(name);
                else
                    stack.Pop();
            }

            return true;
        }

        public override void Set(Value symbol, Value value, StoreMode mode)
        {
            if (!_stacks.TryGetValue(symbol, out var stack))
            {
                stack = new Stack<Value>();

                _stacks[symbol] = stack;
            }

            switch (mode)
            {
                case StoreMode.Global:
                    if (stack.Count > 0)
                        stack.Pop();

                    break;

                case StoreMode.Local:
                    // Lazily create stack frames to current depth
                    while (_depth > _frames.Count)
                        _frames.Push(new HashSet<Value>());

                    // Erase symbol if it previously existed in current stack frame
                    if (_frames.Count > 0 && !_frames.Peek().Add(symbol))
                        stack.Pop();

                    break;
            }

            stack.Push(value);
        }

        public override bool TryGet(Value symbol, out Value value)
        {
            if (_stacks.TryGetValue(symbol, out var stack) && stack.Count > 0)
            {
                value = stack.Peek();

                return true;
            }

            value = default;

            return false;
        }
    }
}