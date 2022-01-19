using System.Collections.Generic;

namespace Cottle.SpyRecords.Evaluable
{
    internal sealed class SpyLookup
    {
        public IEnumerable<Value> Keys => _records.Keys;

        private readonly IDictionary<Value, EvaluableSpyRecord> _records;

        public SpyLookup()
        {
            _records = new Dictionary<Value, EvaluableSpyRecord>();
        }

        public EvaluableSpyRecord CreateOrUpdate(Value key, Value value)
        {
            if (_records.TryGetValue(key, out var record))
                record.Update(value);
            else
            {
                record = new EvaluableSpyRecord(value);

                _records[key] = record;
            }

            return record;
        }

        public EvaluableSpyRecord GetOrCreate(Value key)
        {
            if (!_records.TryGetValue(key, out var record))
            {
                record = new EvaluableSpyRecord(Value.Undefined);

                _records[key] = record;
            }

            return record;
        }
    }
}