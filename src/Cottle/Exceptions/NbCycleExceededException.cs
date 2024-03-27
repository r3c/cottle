using System;
using System.Globalization;

namespace Cottle.Exceptions
{
    public sealed class NbCycleExceededException : Exception
    {
        public int NbCycleMax { get; }

        public NbCycleExceededException(int nbCycleMax) :
            base(string.Format(CultureInfo.InvariantCulture, "exceeded maximum allowed number of cycles '{0}'",
                nbCycleMax))
        {
            NbCycleMax = nbCycleMax;
        }
    }
}