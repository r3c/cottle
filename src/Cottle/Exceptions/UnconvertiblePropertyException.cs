using System;
using System.Reflection;

namespace Cottle.Exceptions
{
    public sealed class UnconvertiblePropertyException : Exception
    {
        public PropertyInfo PropertyInfo { get; }

        public UnconvertiblePropertyException(PropertyInfo propertyInfo, Exception innerException)
            : base($"Unable to create converter for property '{propertyInfo}'", innerException)
        {
            PropertyInfo = propertyInfo;
        }
    }
}