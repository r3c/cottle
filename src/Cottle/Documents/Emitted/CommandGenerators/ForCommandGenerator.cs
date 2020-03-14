using System.Collections.Generic;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.CommandGenerators
{
    internal class ForCommandGenerator : ICommandGenerator
    {
        private readonly ICommandGenerator _body;
        private readonly ICommandGenerator _empty;
        private readonly int? _key;
        private readonly IExpressionGenerator _source;
        private readonly int _value;

        public ForCommandGenerator(IExpressionGenerator source, int? key, int value, ICommandGenerator body,
            ICommandGenerator empty)
        {
            _body = body;
            _empty = empty;
            _key = key;
            _source = source;
            _value = value;
        }

        public bool Generate(Emitter emitter)
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
            var exitRegular = emitter.DeclareLabel();
            var loop = emitter.DeclareLabel();

            emitter.MarkLabel(loop);
            emitter.LoadLocalReference(enumerator);
            emitter.InvokeMapEnumeratorMoveNext();
            emitter.BranchIfFalse(exitRegular);

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
            var exitReturn = emitter.DeclareLabel();
            var mayReturn = false;

            if (_body.Generate(emitter))
            {
                emitter.LoadDuplicate();
                emitter.BranchIfTrue(exitReturn);
                emitter.Discard();

                mayReturn = true;
            }

            emitter.BranchAlways(loop);

            // Evaluate command for "empty" case
            emitter.MarkLabel(empty);

            if (_empty != null)
            {
                if (_empty.Generate(emitter))
                {
                    emitter.LoadDuplicate();
                    emitter.BranchIfTrue(exitReturn);
                    emitter.Discard();

                    mayReturn = true;
                }

                emitter.BranchAlways(exitRegular);
            }

            // End of branch
            emitter.MarkLabel(exitRegular);

            if (mayReturn)
                emitter.LoadBoolean(false);

            // Exit command
            emitter.MarkLabel(exitReturn);

            return mayReturn;
        }
    }
}