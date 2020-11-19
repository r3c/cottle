using System;
using Cottle.Maps;

namespace Cottle.Values
{
    public sealed class VoidValue : BaseValue
    {
        [Obsolete("Use `Value.Undefined`")] public static VoidValue Instance { get; } = new VoidValue();

        public override bool AsBoolean => false;

        public override IFunction AsFunction => Function.Empty;

        public override double AsNumber => 0;

        public override string AsString => string.Empty;

        public override IMap Fields => EmptyMap.Instance;

        public override ValueContent Type => ValueContent.Void;

        public override int CompareTo(Value other)
        {
            if (Type != other.Type)
                return ((int)Type).CompareTo((int)other.Type);

            return 0;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "<void>";
        }
    }
}