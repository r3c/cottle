using System;

namespace Cottle
{
    internal readonly struct RenderConfiguration
    {
        public readonly int? NbCycleMax;

        public RenderConfiguration(int? nbCycleMax)
        {
            NbCycleMax = nbCycleMax;
        }
    }
}