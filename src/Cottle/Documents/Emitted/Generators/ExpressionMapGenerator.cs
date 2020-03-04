using System.Collections.Generic;

namespace Cottle.Documents.Emitted.Generators
{
    internal class ExpressionMapGenerator : IGenerator
    {
        private readonly IReadOnlyList<KeyValuePair<IGenerator, IGenerator>> _elements;

        public ExpressionMapGenerator(IReadOnlyList<KeyValuePair<IGenerator, IGenerator>> elements)
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
                emitter.LoadLocalReference(arguments);
                emitter.LoadInteger(i);
                emitter.LoadElementAddress<KeyValuePair<Value, Value>>();

                // Build pair from key and value
                emitter.LoadLocalReferenceAndRelease(key);
                emitter.LoadLocalReferenceAndRelease(value);
                emitter.NewKeyValuePair();

                // Store pair in arguments[i]
                emitter.StoreValueAtAddress<KeyValuePair<Value, Value>>();
            }

            // Create value from array
            emitter.LoadLocalReferenceAndRelease(arguments);
            emitter.NewMapValue();
        }
    }
}