using System;
using System.Reflection;

namespace Cottle.Exceptions
{
    public sealed class UnconvertiblePropertyException : ReflectionTypeException
    {
        public PropertyInfo PropertyInfo { get; }

        public UnconvertiblePropertyException(PropertyInfo propertyInfo, Exception innerException)
            : base(propertyInfo.PropertyType, innerException)
        {
            PropertyInfo = propertyInfo;
        }
    }
}