using System.Collections.Generic;

namespace Cottle.Documents.Compiled
{
    internal class Scope
    {
        public int LocalCount => _unique;

        private readonly IDictionary<Value, int> _globals;
        private int _localDepth;
        private readonly Stack<HashSet<string>> _localLayers;
        private readonly IDictionary<string, Stack<int>> _localStacks;
        private int _unique;

        public Scope(IDictionary<Value, int> globals)
        {
            _globals = globals;
            _localDepth = 0;
            _localLayers = new Stack<HashSet<string>>();
            _localStacks = new Dictionary<string, Stack<int>>();
            _unique = 0;
        }

        public IReadOnlyList<Value> CreateGlobalNames()
        {
            var names = new Value[_globals.Count];

            foreach (var pair in _globals)
                names[pair.Value] = pair.Key;

            return names;
        }

        public Scope CreateLocalScope()
        {
            return new Scope(_globals);
        }

        public Symbol GetOrDeclareClosest(string name, StoreMode mode)
        {
            if (TryGetLocal(name, out var local))
                return new Symbol(local, StoreMode.Local);

            int index;

            if (mode == StoreMode.Local)
            {
                index = _unique++;

                SetLocal(name, index);
            }
            else if (!_globals.TryGetValue(name, out index))
            {
                index = _globals.Count;

                _globals[name] = index;
            }

            return new Symbol(index, mode);
        }

        public Symbol GetOrDeclareLocal(string name)
        {
            if (TryGetLocal(name, out var local))
                return new Symbol(local, StoreMode.Local);

            var index = _unique++;

            SetLocal(name, index);

            return new Symbol(index, StoreMode.Local);
        }

        public void Enter()
        {
            // Stack frames are lazily built (see "SetLocal" method) so we just keep trace of current frame depth here
            ++_localDepth;
        }

        public void Leave()
        {
            if (_localDepth < 1)
                return;

            // Early exit if no stack frames were allocated at parent depth level
            if (--_localDepth >= _localLayers.Count)
                return;

            // Delete all current stack frame symbols
            foreach (var name in _localLayers.Pop())
            {
                if (!_localStacks.TryGetValue(name, out var stack))
                    continue;

                if (stack.Count < 2)
                    _localStacks.Remove(name);
                else
                    stack.Pop();
            }
        }

        private void SetLocal(string symbol, int local)
        {
            if (!_localStacks.TryGetValue(symbol, out var stack))
            {
                stack = new Stack<int>();

                _localStacks[symbol] = stack;
            }

            // Lazily create stack frames to current depth
            while (_localDepth > _localLayers.Count)
                _localLayers.Push(new HashSet<string>());

            // Erase symbol if it previously existed in current stack frame
            if (_localLayers.Count > 0 && !_localLayers.Peek().Add(symbol))
                stack.Pop();

            stack.Push(local);
        }

        private bool TryGetLocal(string symbol, out int local)
        {
            if (_localStacks.TryGetValue(symbol, out var stack) && stack.Count > 0)
            {
                local = stack.Peek();

                return true;
            }

            local = default;

            return false;
        }
    }
}