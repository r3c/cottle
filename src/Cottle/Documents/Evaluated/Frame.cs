using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Functions;

namespace Cottle.Documents.Evaluated
{
    internal class Frame
    {
        public readonly Value[] Globals;
        public readonly Value[] Locals;

        private Stack<IFunction>? _modifiers;

        public Frame(Value[] globals, int localCount, Stack<IFunction>? modifiers)
        {
            _modifiers = modifiers;

            Globals = globals;
            Locals = localCount > 0 ? new Value[localCount] : Array.Empty<Value>();
        }

        public Frame CreateForFunction(int localCount)
        {
            return new Frame(Globals, localCount, _modifiers);
        }

        public string Echo(Value value, TextWriter output)
        {
            if (_modifiers is null)
                return value.AsString;

            foreach (var modifier in _modifiers)
            {
                if (modifier is FiniteFunction finiteModifier)
                    value = finiteModifier.Invoke1(this, value, output);
                else
                    value = modifier.Invoke(this, new[] { value }, output);
            }

            return value.AsString;
        }

        public IFunction Unwrap()
        {
            if (_modifiers is not null && _modifiers.Count > 0)
                return _modifiers.Pop();

            return Function.Empty;
        }

        public void Wrap(IFunction modifier)
        {
            if (_modifiers is null)
                _modifiers = new Stack<IFunction>();

            _modifiers.Push(modifier);
        }
    }
}