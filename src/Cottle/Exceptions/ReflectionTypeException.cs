using System;

namespace Cottle.Exceptions
{
    public class ReflectionTypeException : Exception
    {
        public Type Type { get; }

        internal ReflectionTypeException(Type type, Exception innerException)
            : base($"Unable to convert type '{type.FullName}'", innerException)
        {
            Type = type;
        }
    }
}