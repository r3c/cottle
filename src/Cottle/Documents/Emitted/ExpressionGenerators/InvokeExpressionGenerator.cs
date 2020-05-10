using System.Collections.Generic;

namespace Cottle.Documents.Emitted.ExpressionGenerators
{
    internal class InvokeExpressionGenerator : IExpressionGenerator
    {
        private readonly IReadOnlyList<IExpressionGenerator> _arguments;
        private readonly IExpressionGenerator _caller;

        public InvokeExpressionGenerator(IExpressionGenerator caller, IReadOnlyList<IExpressionGenerator> arguments)
        {
            _arguments = arguments;
            _caller = caller;
        }

        public void Generate(Emitter emitter)
        {
            // Evaluate source expression as a function
            _caller.Generate(emitter);

            var source = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadLocalAddressAndRelease(source);
            emitter.InvokeValueAsFunction();

            var function = emitter.DeclareLocalAndStore<IFunction>();

            emitter.LoadArray<Value>(_arguments.Count);

            var arguments = emitter.DeclareLocalAndStore<Value[]>();

            // Evaluate arguments one by one and store them into array
            for (var i = 0; i < _arguments.Count; ++i)
            {
                emitter.LoadLocalValue(arguments);
                emitter.LoadInteger(i);

                _arguments[i].Generate(emitter);

                emitter.StoreElementAtIndex<Value>();
            }

            // Invoke function with frame, arguments and output
            emitter.LoadLocalValueAndRelease(function);
            emitter.LoadFrame();
            emitter.LoadLocalValueAndRelease(arguments);
            emitter.LoadOutput();
            emitter.InvokeFunction();
        }
    }
}