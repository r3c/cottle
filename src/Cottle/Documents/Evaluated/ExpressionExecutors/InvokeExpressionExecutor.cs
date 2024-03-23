using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Functions;

namespace Cottle.Documents.Evaluated.ExpressionExecutors
{
    internal class InvokeExpressionExecutor : IExpressionExecutor
    {
        private readonly IReadOnlyList<IExpressionExecutor> _arguments;
        private readonly IExpressionExecutor _caller;

        private readonly Func<Runtime, Frame, FiniteFunction, IReadOnlyList<IExpressionExecutor>, TextWriter, Value>
            _evaluateFinite;

        public InvokeExpressionExecutor(IExpressionExecutor caller, IReadOnlyList<IExpressionExecutor> arguments)
        {
            switch (arguments.Count)
            {
                case 0:
                    _evaluateFinite = InvokeExpressionExecutor.EvaluateFinite0;

                    break;

                case 1:
                    _evaluateFinite = InvokeExpressionExecutor.EvaluateFinite1;

                    break;

                case 2:
                    _evaluateFinite = InvokeExpressionExecutor.EvaluateFinite2;

                    break;

                case 3:
                    _evaluateFinite = InvokeExpressionExecutor.EvaluateFinite3;

                    break;

                default:
                    _evaluateFinite = InvokeExpressionExecutor.EvaluateNothing;

                    break;
            }

            _arguments = arguments;
            _caller = caller;
        }

        public Value Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            var function = _caller.Execute(runtime, frame, output).AsFunction;

            // Try handling function call with known number of arguments
            if (function is FiniteFunction finiteFunction)
                return _evaluateFinite(runtime, frame, finiteFunction, _arguments, output);

            // Fallback to arbitrary number of arguments
            var values = new Value[_arguments.Count];

            for (var i = 0; i < _arguments.Count; ++i)
                values[i] = _arguments[i].Execute(runtime, frame, output);

            return function.Invoke((runtime, frame), values, output);
        }

        private static Value EvaluateFinite0(Runtime runtime, Frame frame, FiniteFunction function,
            IReadOnlyList<IExpressionExecutor> arguments,
            TextWriter output)
        {
            return function.Invoke0((runtime, frame), output);
        }

        private static Value EvaluateFinite1(Runtime runtime, Frame frame, FiniteFunction function,
            IReadOnlyList<IExpressionExecutor> arguments,
            TextWriter output)
        {
            var argument0 = arguments[0].Execute(runtime, frame, output);

            return function.Invoke1((runtime, frame), argument0, output);
        }

        private static Value EvaluateFinite2(Runtime runtime, Frame frame, FiniteFunction function,
            IReadOnlyList<IExpressionExecutor> arguments,
            TextWriter output)
        {
            var argument0 = arguments[0].Execute(runtime, frame, output);
            var argument1 = arguments[1].Execute(runtime, frame, output);

            return function.Invoke2((runtime, frame), argument0, argument1, output);
        }

        private static Value EvaluateFinite3(Runtime runtime, Frame frame, FiniteFunction function,
            IReadOnlyList<IExpressionExecutor> arguments,
            TextWriter output)
        {
            var argument0 = arguments[0].Execute(runtime, frame, output);
            var argument1 = arguments[1].Execute(runtime, frame, output);
            var argument2 = arguments[2].Execute(runtime, frame, output);

            return function.Invoke3((runtime, frame), argument0, argument1, argument2, output);
        }

        private static Value EvaluateNothing(Runtime runtime, Frame frame, FiniteFunction function,
            IReadOnlyList<IExpressionExecutor> arguments,
            TextWriter output)
        {
            return Value.Undefined;
        }
    }
}