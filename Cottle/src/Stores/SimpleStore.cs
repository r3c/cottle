using System.Collections.Generic;

namespace Cottle.Stores
{
    public sealed class SimpleStore : AbstractStore
    {
        #region Constructors

        public SimpleStore()
        {
            _levels = new Stack<HashSet<Value>>();
            _stacks = new Dictionary<Value, Stack<Value>>();
        }

        #endregion

        #region Attributes

        private readonly Stack<HashSet<Value>> _levels;

        private readonly Dictionary<Value, Stack<Value>> _stacks;

        #endregion

        #region Methods

        public override void Enter()
        {
            _levels.Push(new HashSet<Value>());
        }

        public override bool Leave()
        {
            if (_levels.Count < 1)
                return false;

            foreach (var name in _levels.Pop())
                if (_stacks.TryGetValue(name, out var stack))
                {
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
                    if (_levels.Count > 0 && !_levels.Peek().Add(symbol))
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

            value = null;

            return false;
        }

        #endregion
    }
}