using System.Collections.Generic;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandForGenerator : IGenerator
    {
        private readonly IGenerator _body;
        private readonly IGenerator _empty;
        private readonly int? _key;
        private readonly IGenerator _source;
        private readonly int _value;

        public CommandForGenerator(IGenerator source, int? key, int value, IGenerator body,
            IGenerator empty)
        {
            _body = body;
            _empty = empty;
            _key = key;
            _source = source;
            _value = value;
        }

        public void Generate(Emitter emitter)
        {
            // Evaluate operand fields and store as local
            _source.Generate(emitter);

            emitter.InvokeValueFields();

            var fields = emitter.DeclareLocalAndStore<IMap>();

            // Get number of fields and jump to empty statement if count is zero
            var empty = emitter.DeclareLabel();

            emitter.LoadLocalReference(fields);
            emitter.InvokeMapCount();
            emitter.BranchIfFalse(empty);

            // Get fields enumerator and store as local
            emitter.LoadLocalReferenceAndRelease(fields);
            emitter.InvokeMapGetEnumerator();

            var enumerator = emitter.DeclareLocalAndStore<IEnumerator<KeyValuePair<Value, Value>>>();

            // Try moving to next element if any or terminate loop otherwise
            var complete = emitter.DeclareLabel();
            var loop = emitter.DeclareLabel();

            emitter.MarkLabel(loop);
            emitter.LoadLocalReference(enumerator);
            emitter.InvokeMapEnumeratorMoveNext();
            emitter.BranchIfFalse(complete);

            // Fetch current key/value pair and store as local
            emitter.LoadLocalReferenceAndRelease(enumerator);
            emitter.InvokeMapEnumeratorCurrent();

            var pair = emitter.DeclareLocalAndStore<KeyValuePair<Value, Value>>();

            // Set current element key if defined
            if (_key.HasValue)
            {
                emitter.LoadFrameSymbol(new Symbol(_key.Value, StoreMode.Local));
                emitter.LoadLocalValue(pair);
                emitter.InvokePairKey();
                emitter.StoreReferenceAtIndex();
            }

            // Set current element value
            emitter.LoadFrameSymbol(new Symbol(_value, StoreMode.Local));
            emitter.LoadLocalValueAndRelease(pair);
            emitter.InvokePairValue();
            emitter.StoreReferenceAtIndex();

            // Evaluate body and restart cycle
            var halt = emitter.DeclareLabel();

            _body.Generate(emitter);

            emitter.BranchIfTrue(halt);
            emitter.BranchAlways(loop);

            // Evaluate command for "empty" case
            var exit = emitter.DeclareLabel();

            emitter.MarkLabel(empty);

            if (_empty != null)
            {
                _empty.Generate(emitter);

                emitter.BranchAlways(exit);
            }

            // Exit loop with no-return flag
            emitter.MarkLabel(complete);
            emitter.LoadBoolean(false);
            emitter.BranchAlways(exit);

            // Exit loop with return flag
            emitter.MarkLabel(halt);
            emitter.LoadBoolean(true);

            // Exit command
            emitter.MarkLabel(exit);
        }
    }
}