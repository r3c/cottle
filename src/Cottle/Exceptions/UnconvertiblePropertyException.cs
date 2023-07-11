using System;
using System.Reflection;

namespace Cottle.Exceptions {
    public class UnconvertiblePropertyException : Exception {
        public UnconvertiblePropertyException(PropertyInfo propertyInfo, Exception? innerException = null)
            : base($"Unable to create converter for property {propertyInfo}", innerException) {
            PropertyInfo = propertyInfo;
        }

        public PropertyInfo PropertyInfo { get; set; }
    }
}