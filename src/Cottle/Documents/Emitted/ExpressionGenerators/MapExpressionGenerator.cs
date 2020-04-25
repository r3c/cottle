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
            emitter.LoadArray<KeyValuePair<Value, Value>>(_elements.Count);

            var arguments = emitter.DeclareLocalAndStore<KeyValuePair<Value, Value>[]>();

            // Evaluate elements and store into array
            for (var i = 0; i < _elements.Count; ++i)
            {
                _elements[i].Key.Generate(emitter);

                var key = emitter.DeclareLocalAndStore<Value>();

                _elements[i].Value.Generate(emitter);

                var value = emitter.DeclareLocalAndStore<Value>();

                // Load address of arguments[i]
                emitter.LoadLocalValue(arguments);
                emitter.LoadInteger(i);
                emitter.LoadElementAddress<KeyValuePair<Value, Value>>();

                // Build pair from key and value
                emitter.LoadLocalValueAndRelease(key);
                emitter.LoadLocalValueAndRelease(value);
                emitter.NewKeyValuePair();

                // Store pair in arguments[i]
                emitter.StoreValueAtAddress<KeyValuePair<Value, Value>>();
            }

            // Create value from array
            emitter.LoadLocalValueAndRelease(arguments);
            emitter.InvokeValueFromDictionary();
        }
    }
}