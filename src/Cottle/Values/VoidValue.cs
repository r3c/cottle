﻿using Cottle.Maps;

namespace Cottle.Values
{
    public sealed class VoidValue : Value
    {
        public static VoidValue Instance { get; } = new VoidValue();

        public override bool AsBoolean => false;

        public override IFunction AsFunction => null;

        public override decimal AsNumber => 0;

        public override string AsString => string.Empty;

        public override IMap Fields => EmptyMap.Instance;

        public override ValueContent Type => ValueContent.Void;

        public override int CompareTo(Value other)
        {
            if (other == null)
                return 1;

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