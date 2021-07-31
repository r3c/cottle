using System.Collections.Generic;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class ForStatementGenerator : IStatementGenerator
    {
        private readonly IStatementGenerator _body;
        private readonly IStatementGenerator? _empty;
        private readonly Symbol? _key;
        private readonly IExpressionGenerator _source;
        private readonly Symbol _value;

        public ForStatementGenerator(IExpressionGenerator source, Symbol? key, Symbol value, IStatementGenerator body,
            IStatementGenerator? empty)
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

            var source = emitter.EmitDeclareLocalAndStore<Value>();

            emitter.EmitLoadLocalAddressAndRelease(source);
            emitter.EmitCallValueFields();

            var fields = emitter.EmitDeclareLocalAndStore<IMap>();

            // Get number of fields and jump to empty statement if count is zero
            var empty = emitter.DeclareLabel();

            emitter.EmitLoadLocalValue(fields);
            emitter.EmitCallMapCount();
            emitter.EmitBranchWhenFalse(empty);

            // Get fields enumerator and store as local
            emitter.EmitLoadLocalValueAndRelease(fields);
            emitter.EmitCallMapGetEnumerator();

            var enumerator = emitter.EmitDeclareLocalAndStore<IEnumerator<KeyValuePair<Value, Value>>>();

            // Try moving to next element if any or terminate loop otherwise
            var exitRegular = emitter.DeclareLabel();
            var loop = emitter.DeclareLabel();

            emitter.MarkLabel(loop);
            emitter.EmitLoadLocalValue(enumerator);
            emitter.EmitCallMapEnumeratorMoveNext();
            emitter.EmitBranchWhenFalse(exitRegular);

            // Fetch current key/value pair and store as local
            emitter.EmitLoadLocalValueAndRelease(enumerator);
            emitter.EmitCallMapEnumeratorCurrent();

            var pair = emitter.EmitDeclareLocalAndStore<KeyValuePair<Value, Value>>();

            // Set current element key if defined
            if (_key.HasValue)
            {
                emitter.EmitLoadLocalAddress(pair);
                emitter.EmitCallPairKey();
                emitter.EmitStoreLocal(emitter.GetOrDeclareSymbol(_key.Value.Index));
            }

            // Set current element value
            emitter.EmitLoadLocalAddressAndRelease(pair);
            emitter.EmitCallPairValue();
            emitter.EmitStoreLocal(emitter.GetOrDeclareSymbol(_value.Index));

            // Evaluate body and restart cycle
            var exitReturn = emitter.DeclareLabel();
            var mayReturn = false;

            if (_body.Generate(emitter))
            {
                emitter.EmitLoadDuplicate();
                emitter.EmitBranchWhenTrue(exitReturn);
                emitter.EmitDiscard();

                mayReturn = true;
            }

            emitter.EmitBranchAlways(loop);

            // Evaluate statement for "empty" case
            emitter.MarkLabel(empty);

            if (_empty != null)
            {
                if (_empty.Generate(emitter))
                {
                    emitter.EmitLoadDuplicate();
                    emitter.EmitBranchWhenTrue(exitReturn);
                    emitter.EmitDiscard();

                    mayReturn = true;
                }

                emitter.EmitBranchAlways(exitRegular);
            }

            // End of branch
            emitter.MarkLabel(exitRegular);

            if (mayReturn)
                emitter.EmitLoadBoolean(false);

            // Exit statement
            emitter.MarkLabel(exitReturn);

            return mayReturn;
        }
    }
}