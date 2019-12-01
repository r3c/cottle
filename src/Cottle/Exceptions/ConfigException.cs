using System;
using System.Globalization;

namespace Cottle.Exceptions
{
    public sealed class ConfigException : Exception
    {
        public string Name { get; }

        public string Value { get; }

        public ConfigException(string name, string value, string message) :
            base(string.Format(CultureInfo.InvariantCulture, "{2} (option '{0}', value '{1}')", name, value, message))
        {
            Name = name;
            Value = value;
        }
    }
}