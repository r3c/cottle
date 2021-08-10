using System;
using System.Collections.Generic;
using Cottle.Functions;

namespace Cottle.Documents.Emitted.ExpressionGenerators
{
    internal class InvokeExpressionGenerator : IExpressionGenerator
    {
        private readonly IReadOnlyList<IExpressionGenerator> _arguments;
        private readonly IExpressionGenerator _caller;
        private readonly Action<Emitter>? _finiteFunctionInvoke;

        public InvokeExpressionGenerator(IExpressionGenerator caller, IReadOnlyList<IExpressionGenerator> arguments)
        {
            switch (arguments.Count)
            {
                case 0:
                    _finiteFunctionInvoke = e => e.EmitCallFiniteFunctionInvoke0();

                    break;

                case 1:
                    _finiteFunctionInvoke = e => e.EmitCallFiniteFunctionInvoke1();

                    break;

                case 2:
                    _finiteFunctionInvoke = e => e.EmitCallFiniteFunctionInvoke2();

                    break;

                case 3:
                    _finiteFunctionInvoke = e => e.EmitCallFiniteFunctionInvoke3();

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

            var caller = emitter.EmitDeclareLocalAndStore<Value>();

            emitter.EmitLoadLocalAddressAndRelease(caller);
            emitter.EmitCallValueAsFunction();

            var function = emitter.EmitDeclareLocalAndStore<IFunction>();

            // Evaluate arguments and store as local variables
            var argumentsLocals = new Local<Value>[_arguments.Count];

            for (var i = 0; i < _arguments.Count; ++i)
            {
                _arguments[i].Generate(emitter);

                argumentsLocals[i] = emitter.EmitDeclareLocalAndStore<Value>();
            }

            // Try to cast function as a finite function instance and call with fixed number of arguments
            emitter.EmitLoadLocalValue(function);
            emitter.EmitCastAs<FiniteFunction>();

            var finiteFunction = emitter.EmitDeclareLocalAndStore<FiniteFunction>();
            var arbitrary = emitter.DeclareLabel();

            emitter.EmitLoadLocalValue(finiteFunction);
            emitter.EmitBranchWhenFalse(arbitrary);

            // Perform call with known number of arguments if possible
            if (_finiteFunctionInvoke != null)
            {
                emitter.EmitLoadLocalValueAndRelease(finiteFunction);
                emitter.EmitLoadFrame();

                for (var i = 0; i < _arguments.Count; ++i)
                    emitter.EmitLoadLocalValue(argumentsLocals[i]);

                emitter.EmitLoadOutput();

                _finiteFunctionInvoke(emitter);
            }
            else
                emitter.EmitLoadUndefined();

            var exit = emitter.DeclareLabel();

            emitter.EmitBranchAlways(exit);

            // Fallback to arbitrary number of arguments
            emitter.MarkLabel(arbitrary);
            emitter.EmitLoadArray<Value>(_arguments.Count);

            var argumentArray = emitter.EmitDeclareLocalAndStore<Value[]>();

            // Evaluate arguments one by one and store them into array
            for (var i = 0; i < _arguments.Count; ++i)
            {
                emitter.EmitLoadLocalValue(argumentArray);
                emitter.EmitLoadInteger(i);
                emitter.EmitLoadLocalValueAndRelease(argumentsLocals[i]);
                emitter.EmitStoreElementAtIndex<Value>();
            }

            // Invoke function with frame, arguments and output
            emitter.EmitLoadLocalValueAndRelease(function);
            emitter.EmitLoadFrame();
            emitter.EmitLoadLocalValueAndRelease(argumentArray);
            emitter.EmitLoadOutput();
            emitter.EmitCallFunctionInvoke();

            // Exit and return
            emitter.MarkLabel(exit);
        }
    }
}