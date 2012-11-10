using System;

namespace   Cottle
{
    public enum ValueContent
    {
        Map,
        Boolean,
        Function,
        Number,
        String,
        Undefined,

        [Obsolete("Please use Map value")]
        Array = 0
    }
}
