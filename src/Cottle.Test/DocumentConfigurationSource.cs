using System.Collections.Generic;

namespace Cottle.Test
{
    internal static class DocumentConfigurationSource
    {
        public static readonly IReadOnlyList<DocumentConfiguration> Configurations = new[]
        {
            new DocumentConfiguration { NoOptimize = false },
            new DocumentConfiguration { NoOptimize = true }
        };
    }
}