using System.Collections.Generic;

namespace Cottle.Demo.Serialization
{
    internal readonly struct State
    {
        public readonly DocumentConfiguration Configuration;
        public readonly string Template;
        public readonly IReadOnlyDictionary<string, Value> Values;

        public State(DocumentConfiguration configuration, IReadOnlyDictionary<string, Value> values, string template)
        {
            Configuration = configuration;
            Template = template;
            Values = values;
        }
    }
}