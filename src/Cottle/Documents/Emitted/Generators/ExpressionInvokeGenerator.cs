using System.Collections.Generic;

namespace Cottle.Documents.Emitted.Generators
{
    internal class ExpressionInvokeGenerator : IGenerator
    {
        private readonly IReadOnlyList<IGenerator> _arguments;
        private readonly IGenerator _caller;

        public ExpressionInvokeGenerator(IGenerator caller, IReadOnlyList<IGenerator> argumentses)
        {
            _arguments = argumentses;
            _caller = caller;
        }

        public void Generate(Emitter emitter)
        {
            // Evaluate source expression as a function
            _caller.Generate(emitter);

            emitter.InvokeValueAsFunction();

            var function = emitter.DeclareLocalAndStore<IFunction>();

            emitter.LoadLocalReference(function);

            var invoke = emitter.DeclareLabel();

            emitter.BranchIfTrue(invoke);

            // Emit void value on error
            var exit = emitter.DeclareLabel();

            emitter.LoadVoid();
            emitter.BranchAlways(exit);

            // Create array to store evaluated values
            emitter.MarkLabel(invoke);
            emitter.LoadArray<Value>(_arguments.Count);

            var arguments = emitter.DeclareLocalAndStore<Value[]>();

            // Evaluate arguments one by one and store them into array
            for (var i = 0; i < _arguments.Count; ++i)
            {
                _arguments[i].Generate(emitter);

                var argument = emitter.DeclareLocalAndStore<Value>();

                emitter.LoadLocalReference(arguments);
                emitter.LoadInteger(i);
                emitter.LoadLocalReferenceAndRelease(argument);
                emitter.StoreReferenceAtIndex();
            }

            // Invoke function with frame, arguments and output
            emitter.LoadLocalReferenceAndRelease(function);
            emitter.LoadFrame();
            emitter.LoadLocalReferenceAndRelease(arguments);
            emitter.LoadOutput();
            emitter.InvokeFunction();

            // Value is already available on stack
            emitter.MarkLabel(exit);
        }
    }
}