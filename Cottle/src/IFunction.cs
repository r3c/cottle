using System;
using System.Collections.Generic;
using System.IO;

namespace Cottle
{
    public interface IFunction : IComparable<IFunction>, IEquatable<IFunction>
    {
        Value Execute(IList<Value> arguments, IStore store, TextWriter output);

        string ToString();
    }
}