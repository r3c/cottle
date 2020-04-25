using System;
using System.Reflection;
using Cottle.Evaluables;

namespace Cottle.Values
{
    public sealed class ReflectionValue : ResolveValue
    {
        private readonly BindingFlags _bindingFlags;
        private readonly object _source;

        [Obsolete("Use `Value.FromReflection()`")]
        public ReflectionValue(object source, BindingFlags binding)
        {
            _bindingFlags = binding;
            _source = source;
        }

        [Obsolete("Use `Value.FromReflection()`")]
        public ReflectionValue(object source) :
            this(source, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
        }

        protected override Value Resolve()
        {
            return ReflectionEvaluable.CreateValue(_source, _bindingFlags);
        }
    }
}