using System;

namespace Cottle
{
    internal readonly struct RenderConfiguration
    {
        public readonly TimeSpan? Timeout;

        public RenderConfiguration(TimeSpan? timeout)
        {
            Timeout = timeout;
        }
    }
}