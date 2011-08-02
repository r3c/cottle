using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle
{
    public delegate IValue  Function (Scope scope, IList<IValue> arguments);
}
