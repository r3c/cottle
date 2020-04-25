using System;

namespace Cottle
{
    public interface IEvaluable : IComparable<Value>, IEquatable<Value>
    {
        bool AsBoolean { get; }
        IFunction AsFunction { get; }
        double AsNumber { get; }
        string AsString { get; }
        IMap Fields { get; }
        ValueContent Type { get; }
    }
}