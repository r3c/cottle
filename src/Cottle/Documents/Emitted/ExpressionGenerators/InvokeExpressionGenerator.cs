using System;
using System.Collections.Generic;
using Cottle.Functions;

namespace Cottle.Documents.Emitted.ExpressionGenerators
{
    internal class InvokeExpressionGenerator : IExpressionGenerator
    {
        private readonly IReadOnlyList<IExpressionGenerator> _arguments;
        private readonly IExpressionGenerator _caller;
        private readonly Action<Emitter> _finiteFunctionInvoke;

        public InvokeExpressionGenerator(IExpressionGenerator caller, IReadOnlyList<IExpressionGenerator> arguments)
        {
            switch (arguments.Count)
            {
                case 0:
                    _finiteFunctionInvoke = e => e.InvokeFiniteFunctionInvoke0();

                    break;

                case 1:
                    _finiteFunctionInvoke = e => e.InvokeFiniteFunctionInvoke1();

                    break;

                case 2:
                    _finiteFunctionInvoke = e => e.InvokeFiniteFunctionInvoke2();

                    break;

                case 3:
                    _finiteFunctionInvoke = e => e.InvokeFiniteFunctionInvoke3();

                    break;

                default:
                    _finiteFunctionInvoke = null;

                    break;
            }

            _arguments = arguments;
            _caller = caller;
        }

        public void Generate(Emitter emitter)
        {
            // Evaluate source expression as a function
            _caller.Generate(emitter);

            var caller = emitter.DeclareLocalAndStore<Value>();

            emitter.LoadLocalAddressAndRelease(caller);
            emitter.InvokeValueAsFunction();

            var function = emitter.DeclareLocalAndStore<IFunction>();

            // Evaluate arguments and store as local variables
            var argumentsLocals = new Local<Value>[_arguments.Count];

            for (var i = 0; i < _arguments.Count; ++i)
            {
                _arguments[i].Generate(emitter);

                argumentsLocals[i] = emitter.DeclareLocalAndStore<Value>();
            }

            // Emit code for calls with with fixed number of arguments if compatible
            var exit = emitter.DeclareLabel();

            if (_finiteFunctionInvoke != null)
            {
                // Try to cast function as a finite function instance
                emitter.LoadLocalValue(function);
                emitter.CastAs<FiniteFunction>();

                var finiteFunction = emitter.DeclareLocalAndStore<FiniteFunction>();
                var arbitrary = emitter.DeclareLabel();

                emitter.LoadLocalValue(finiteFunction);
                emitter.BranchIfFalse(arbitrary);

                // Perform call with known number of arguments
                emitter.LoadLocalValueAndRelease(finiteFunction);
                emitter.LoadFrame();

                for (var i = 0; i < _arguments.Count; ++i)
                    emitter.LoadLocalValue(argumentsLocals[i]);

                emitter.LoadOutput();

                _finiteFunctionInvoke(emitter);

                emitter.BranchAlways(exit);
                emitter.MarkLabel(arbitrary);
            }

            // Fallback to arbitrary number of arguments
            emitter.LoadArray<Value>(_arguments.Count);

            var argumentArray = emitter.DeclareLocalAndStore<Value[]>();

            // Evaluate arguments one by one and store them into array
            for (var i = 0; i < _arguments.Count; ++i)
            {
                emitter.LoadLocalValue(argumentArray);
                emitter.LoadInteger(i);
                emitter.LoadLocalValueAndRelease(argumentsLocals[i]);
                emitter.StoreElementAtIndex<Value>();
            }

            // Invoke function with frame, arguments and output
            emitter.LoadLocalValueAndRelease(function);
            emitter.LoadFrame();
            emitter.LoadLocalValueAndRelease(argumentArray);
            emitter.LoadOutput();
            emitter.InvokeFunctionInvoke();

            // Exit and return
            emitter.MarkLabel(exit);
        }
    }
}