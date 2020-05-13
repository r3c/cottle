using System.Collections.Generic;

namespace Cottle.Documents.Emitted.ExpressionGenerators
{
    internal class MapExpressionGenerator : IExpressionGenerator
    {
        private readonly IReadOnlyList<KeyValuePair<IExpressionGenerator, IExpressionGenerator>> _elements;

        public MapExpressionGenerator(IReadOnlyList<KeyValuePair<IExpressionGenerator, IExpressionGenerator>> elements)
        {
            _elements = elements;
        }

        public void Generate(Emitter emitter)
        {
            // Create array to store evaluated pairs
            emitter.EmitLoadArray<KeyValuePair<Value, Value>>(_elements.Count);

            var arguments = emitter.EmitDeclareLocalAndStore<KeyValuePair<Value, Value>[]>();

            // Evaluate elements and store into array
            for (var i = 0; i < _elements.Count; ++i)
            {
                _elements[i].Key.Generate(emitter);

                var key = emitter.EmitDeclareLocalAndStore<Value>();

                _elements[i].Value.Generate(emitter);

                var value = emitter.EmitDeclareLocalAndStore<Value>();

                // Load address of arguments[i]
                emitter.EmitLoadLocalValue(arguments);
                emitter.EmitLoadInteger(i);
                emitter.EmitLoadElementAddress<KeyValuePair<Value, Value>>();

                // Build pair from key and value
                emitter.EmitLoadLocalValueAndRelease(key);
                emitter.EmitLoadLocalValueAndRelease(value);
                emitter.EmitNewKeyValuePair();

                // Store pair in arguments[i]
                emitter.EmitStoreValueAtAddress<KeyValuePair<Value, Value>>();
            }

            // Create value from array
            emitter.EmitLoadLocalValueAndRelease(arguments);
            emitter.EmitCallValueFromDictionary();
        }
    }
}