﻿using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Default.Executors
{
    internal class ForExecutor : IExecutor
    {
        private readonly IExecutor _body;

        private readonly IExecutor _empty;

        private readonly IEvaluator _from;

        private readonly int? _key;

        private readonly int _value;

        public ForExecutor(IEvaluator from, int? key, int value, IExecutor body, IExecutor empty)
        {
            _body = body;
            _empty = empty;
            _from = from;
            _key = key;
            _value = value;
        }

        public bool Execute(Stack stack, TextWriter output, out Value result)
        {
            var fields = _from.Evaluate(stack, output).Fields;

            if (fields.Count > 0)
            {
                foreach (var pair in fields)
                {
                    if (_key.HasValue)
                        stack.Locals[_key.Value] = pair.Key;

                    stack.Locals[_value] = pair.Value;

                    if (_body.Execute(stack, output, out result))
                        return true;
                }
            }
            else if (_empty != null && _empty.Execute(stack, output, out result))
                return true;

            result = VoidValue.Instance;

            return false;
        }
    }
}