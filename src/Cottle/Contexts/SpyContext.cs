using System.Collections.Generic;
using Cottle.SpyRecords.Evaluable;

namespace Cottle.Contexts
{
    internal sealed class SpyContext : ISpyContext
    {
        private readonly SpyLookup _lookup;

        private readonly IContext _source;

        public SpyContext(IContext source)
        {
            _lookup = new SpyLookup();
            _source = source;
        }

        public Value this[Value symbol]
        {
            get
            {
                var initial = _source[symbol];
                var record = _lookup.CreateOrUpdate(symbol, initial);

                return Value.FromEvaluable(record);
            }
        }

        public ISpyRecord SpyVariable(Value key)
        {
            return _lookup.GetOrCreate(key);
        }

        public IReadOnlyDictionary<Value, ISpyRecord> SpyVariables()
        {
            var fields = new Dictionary<Value, ISpyRecord>();

            foreach (var key in _lookup.Keys)
                fields[key] = _lookup.GetOrCreate(key);

            return fields;
        }
    }
}