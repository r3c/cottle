using System;
using System.Reflection;

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
            return Value.FromReflection(_source, _bindingFlags);
        }
    }
}