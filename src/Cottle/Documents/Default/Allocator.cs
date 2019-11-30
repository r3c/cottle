using System.Collections.Generic;
using Cottle.Stores;

namespace Cottle.Documents.Default
{
    internal class Allocator
    {
        private readonly IDictionary<Value, int> _globals;
        private readonly IStore _locals;
        private int _unique;

        public Allocator(IDictionary<Value, int> globals)
        {
            _globals = globals;
            _locals = new SimpleStore();
            _unique = 0;
        }

        public int DeclareAsLocal(Value name)
        {
            if (_locals.TryGet(name, out var local))
                return (int)local.AsNumber;

            var index = _unique++;

            _locals.Set(name, index, StoreMode.Local);

            return index;
        }

        public Allocator EnterFunction()
        {
            return new Allocator(_globals);
        }

        public void EnterScope()
        {
            _locals.Enter();
        }

        public Symbol FindOrDeclare(Value name, StoreMode mode)
        {
            if (_locals.TryGet(name, out var local))
                return new Symbol((int)local.AsNumber, StoreMode.Local);

            int index;

            if (mode == StoreMode.Global)
            {
                if (!_globals.TryGetValue(name, out index))
                {
                    index = _globals.Count;

                    _globals[name] = index;
                }
            }
            else
            {
                index = _unique++;

                _locals.Set(name, index, StoreMode.Local);
            }

            return new Symbol(index, mode);
        }

        public IReadOnlyList<Value> GetGlobals()
        {
            var names = new Value[_globals.Count];

            foreach (var pair in _globals)
                names[pair.Value] = pair.Key;

            return names;
        }

        public int GetLocalCount()
        {
            return _unique;
        }

        public void LeaveScope()
        {
            _locals.Leave();
        }
    }
}