using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Cottle.Evaluables;
using Cottle.Functions;
using Cottle.Maps;
using Cottle.Values;

namespace Cottle
{
    public readonly struct Value : IComparable<Value>, IEquatable<Value>
    {
        private const ValueContent ContentEvaluable = (ValueContent)(-1);

        public static readonly Value EmptyMap = new Value(Maps.EmptyMap.Instance);
        public static readonly Value EmptyString = new Value(string.Empty);
        public static readonly Value False = new Value(false);
        public static readonly Value True = new Value(true);
        public static readonly Value Undefined = new Value();
        public static readonly Value Zero = new Value(0);

        public static bool operator ==(Value lhs, Value rhs)
        {
            return lhs.CompareTo(rhs) == 0;
        }

        public static bool operator !=(Value lhs, Value rhs)
        {
            return lhs.CompareTo(rhs) != 0;
        }

        public static bool operator <(Value lhs, Value rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator <=(Value lhs, Value rhs)
        {
            return lhs.CompareTo(rhs) <= 0;
        }

        public static bool operator >(Value lhs, Value rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool operator >=(Value lhs, Value rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
        }

        public static implicit operator Value(Func<Value> resolver)
        {
            return Value.FromLazy(resolver);
        }

        public static implicit operator Value(Dictionary<Value, Value> dictionary)
        {
            return Value.FromDictionary(dictionary);
        }

        public static implicit operator Value(List<KeyValuePair<Value, Value>> pairs)
        {
            return Value.FromEnumerable(pairs);
        }

        public static implicit operator Value(KeyValuePair<Value, Value>[] pairs)
        {
            return Value.FromEnumerable(pairs);
        }

        public static implicit operator Value(List<Value> elements)
        {
            return Value.FromEnumerable(elements);
        }

        public static implicit operator Value(Value[] elements)
        {
            return Value.FromEnumerable(elements);
        }

        public static implicit operator Value(NativeFunction function)
        {
            return Value.FromFunction(function);
        }

        public static implicit operator Value(bool value)
        {
            return Value.FromBoolean(value);
        }

        public static implicit operator Value(byte value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(char value)
        {
            return Value.FromCharacter(value);
        }

        public static implicit operator Value(decimal value)
        {
            return Value.FromNumber((double)value);
        }

        public static implicit operator Value(double value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(float value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(int value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(long value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(sbyte value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(short value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(string value)
        {
            return Value.FromString(value);
        }

        public static implicit operator Value(ushort value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(uint value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(ulong value)
        {
            return Value.FromNumber(value);
        }

        public static implicit operator Value(BaseValue value)
        {
            return Value.FromEvaluable(value);
        }

        public static Value FromBoolean(bool value)
        {
            return value ? Value.True : Value.False;
        }

        public static Value FromCharacter(char value)
        {
            return new Value(new string(value, 1));
        }

        public static Value FromDictionary(IReadOnlyDictionary<Value, Value>? dictionary)
        {
            return new Value(dictionary);
        }

        public static Value FromEnumerable(IEnumerable<KeyValuePair<Value, Value>>? pairs)
        {
            return new Value(pairs);
        }

        public static Value FromEnumerable(IEnumerable<Value>? elements)
        {
            return new Value(elements);
        }

        public static Value FromEvaluable(IEvaluable? evaluable)
        {
            return new Value(evaluable);
        }

        public static Value FromFunction(IFunction? function)
        {
            return new Value(function);
        }

        public static Value FromGenerator(Func<int, Value>? generator, int count)
        {
            return new Value(generator, count);
        }

        public static Value FromLazy(Func<Value>? resolver)
        {
            return resolver is not null ? new Value(new LazyEvaluable(resolver)) : Value.Undefined;
        }

        public static Value FromMap(IMap? map)
        {
            return new Value(map);
        }

        public static Value FromNumber(byte value)
        {
            return new Value(value);
        }

        public static Value FromNumber(double value)
        {
            return new Value(value);
        }

        public static Value FromNumber(float value)
        {
            return new Value(value);
        }

        public static Value FromNumber(int value)
        {
            return new Value(value);
        }

        public static Value FromNumber(long value)
        {
            return new Value(value);
        }

        public static Value FromNumber(sbyte value)
        {
            return new Value(value);
        }

        public static Value FromNumber(short value)
        {
            return new Value(value);
        }

        public static Value FromNumber(ushort value)
        {
            return new Value(value);
        }

        public static Value FromNumber(uint value)
        {
            return new Value(value);
        }

        public static Value FromNumber(ulong value)
        {
            return new Value(value);
        }

        public static Value FromReflection<TSource>(TSource source, BindingFlags bindingFlags)
        {
            return ReflectionEvaluable.CreateValue(source, bindingFlags);
        }

        public static Value FromString(string? value)
        {
            return new Value(value);
        }

        public bool AsBoolean
        {
            get
            {
                switch (_type)
                {
                    case Value.ContentEvaluable:
                        return _unionReference.Evaluable.AsBoolean;

                    case ValueContent.Boolean:
                        return _unionValue.Boolean;

                    case ValueContent.Map:
                        return Fields.Count > 0;

                    case ValueContent.Number:
                        return Math.Abs(_unionValue.Number) > double.Epsilon;

                    case ValueContent.String:
                        return !string.IsNullOrEmpty(_unionReference.String);

                    case ValueContent.Function:
                    case ValueContent.Void:
                        return false;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public IFunction AsFunction
        {
            get
            {
                switch (_type)
                {
                    case Value.ContentEvaluable:
                        return _unionReference.Evaluable.AsFunction;

                    case ValueContent.Boolean:
                    case ValueContent.Map:
                    case ValueContent.Number:
                    case ValueContent.String:
                    case ValueContent.Void:
                        return Function.Empty;

                    case ValueContent.Function:
                        return _unionReference.Function;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public double AsNumber
        {
            get
            {
                switch (_type)
                {
                    case Value.ContentEvaluable:
                        return _unionReference.Evaluable.AsNumber;

                    case ValueContent.Boolean:
                        return _unionValue.Boolean ? 1 : 0;

                    case ValueContent.Map:
                        return _unionReference.Map.Count;

                    case ValueContent.Number:
                        return _unionValue.Number;

                    case ValueContent.String:
                        return double.TryParse(_unionReference.String, NumberStyles.Number,
                            CultureInfo.InvariantCulture, out var number)
                            ? number
                            : 0;

                    case ValueContent.Function:
                    case ValueContent.Void:
                        return 0;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public string AsString
        {
            get
            {
                switch (_type)
                {
                    case Value.ContentEvaluable:
                        return _unionReference.Evaluable.AsString;

                    case ValueContent.Boolean:
                        return _unionValue.Boolean ? "true" : string.Empty;

                    case ValueContent.Number:
                        return _unionValue.Number.ToString(CultureInfo.InvariantCulture);

                    case ValueContent.String:
                        return _unionReference.String;

                    case ValueContent.Function:
                    case ValueContent.Map:
                    case ValueContent.Void:
                        return string.Empty;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public IMap Fields
        {
            get
            {
                switch (_type)
                {
                    case Value.ContentEvaluable:
                        return _unionReference.Evaluable.Fields;

                    case ValueContent.Map:
                        return _unionReference.Map;

                    case ValueContent.Boolean:
                    case ValueContent.Function:
                    case ValueContent.Number:
                    case ValueContent.String:
                    case ValueContent.Void:
                        return Maps.EmptyMap.Instance;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UnionReference
        {
            [FieldOffset(0)] public IFunction Function;

            [FieldOffset(0)] public IEvaluable Evaluable;

            [FieldOffset(0)] public IMap Map;

            [FieldOffset(0)] public string String;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UnionValue
        {
            [FieldOffset(0)] public bool Boolean;

            [FieldOffset(0)] public double Number;
        }

        public ValueContent Type => _type == Value.ContentEvaluable ? _unionReference.Evaluable.Type : _type;

        private readonly ValueContent _type;

        private readonly UnionReference _unionReference;

        private readonly UnionValue _unionValue;

        private Value(IEvaluable? value)
        {
            if (value == null)
            {
                _type = ValueContent.Void;
                _unionReference = default;
                _unionValue = default;
            }
            else
            {
                _type = Value.ContentEvaluable;
                _unionReference = new UnionReference { Evaluable = value };
                _unionValue = default;
            }
        }

        private Value(IFunction? value)
        {
            if (value == null)
            {
                _type = ValueContent.Void;
                _unionReference = default;
                _unionValue = default;
            }
            else
            {
                _type = ValueContent.Function;
                _unionReference = new UnionReference { Function = value };
                _unionValue = default;
            }
        }

        private Value(IReadOnlyDictionary<Value, Value>? value)
        {
            if (value == null)
            {
                _type = ValueContent.Void;
                _unionReference = default;
                _unionValue = default;
            }
            else
            {
                _type = ValueContent.Map;
                _unionReference = new UnionReference { Map = new HashMap(value) };
                _unionValue = default;
            }
        }

        private Value(IEnumerable<KeyValuePair<Value, Value>>? value)
        {
            if (value == null)
            {
                _type = ValueContent.Void;
                _unionReference = default;
                _unionValue = default;
            }
            else
            {
                _type = ValueContent.Map;
                _unionReference = new UnionReference { Map = new MixMap(value) };
                _unionValue = default;
            }
        }

        private Value(IEnumerable<Value>? value)
        {
            if (value == null)
            {
                _type = ValueContent.Void;
                _unionReference = default;
                _unionValue = default;
            }
            else
            {
                _type = ValueContent.Map;
                _unionReference = new UnionReference { Map = new ArrayMap(value) };
                _unionValue = default;
            }
        }

        private Value(IMap? value)
        {
            if (value == null)
            {
                _type = ValueContent.Void;
                _unionReference = default;
                _unionValue = default;
            }
            else
            {
                _type = ValueContent.Map;
                _unionReference = new UnionReference { Map = value };
                _unionValue = default;
            }
        }

        private Value(Func<int, Value>? generator, int count)
        {
            if (generator == null)
            {
                _type = ValueContent.Void;
                _unionReference = default;
                _unionValue = default;
            }
            else
            {
                _type = ValueContent.Map;
                _unionReference = new UnionReference { Map = new GeneratorMap(generator, count) };
                _unionValue = default;
            }
        }

        private Value(bool value)
        {
            _type = ValueContent.Boolean;
            _unionReference = default;
            _unionValue = new UnionValue { Boolean = value };
        }

        private Value(double value)
        {
            _type = ValueContent.Number;
            _unionReference = default;
            _unionValue = new UnionValue { Number = value };
        }

        private Value(string? value)
        {
            if (value == null)
            {
                _type = ValueContent.Void;
                _unionReference = default;
                _unionValue = default;
            }
            else
            {
                _type = ValueContent.String;
                _unionReference = new UnionReference { String = value };
                _unionValue = default;
            }
        }

        public int CompareTo(Value other)
        {
            var type = Type;

            if (type != other.Type)
                return type.CompareTo(other.Type);

            switch (_type)
            {
                case Value.ContentEvaluable:
                    return _unionReference.Evaluable.CompareTo(other);

                case ValueContent.Boolean:
                    return _unionValue.Boolean.CompareTo(other.AsBoolean);

                case ValueContent.Function:
                    return _unionReference.Function.CompareTo(other.AsFunction);

                case ValueContent.Map:
                    return _unionReference.Map.CompareTo(other.Fields);

                case ValueContent.Number:
                    return _unionValue.Number.CompareTo(other.AsNumber);

                case ValueContent.String:
                    return string.CompareOrdinal(_unionReference.String, other.AsString);

                case ValueContent.Void:
                    return 0;

                default:
                    throw new InvalidOperationException();
            }
        }

        public bool Equals(Value other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is Value other && CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            const int shift = 4;
            const int mask = (1 << shift) - 1;

            switch (_type)
            {
                case Value.ContentEvaluable:
                    return _unionReference.Evaluable.GetHashCode();

                case ValueContent.Boolean:
                    return (_unionValue.Boolean.GetHashCode() << shift) | ((int)ValueContent.Boolean & mask);

                case ValueContent.Function:
                    return (_unionReference.Function.GetHashCode() << shift) | ((int)ValueContent.Function & mask);

                case ValueContent.Map:
                    return (_unionReference.Map.GetHashCode() << shift) | ((int)ValueContent.Map & mask);

                case ValueContent.Number:
                    return (_unionValue.Number.GetHashCode() << shift) | ((int)ValueContent.Number & mask);

                case ValueContent.String:
                    return (_unionReference.String.GetHashCode() << shift) | ((int)ValueContent.String & mask);

                case ValueContent.Void:
                    return Type.GetHashCode();

                default:
                    throw new InvalidOperationException();
            }
        }

        public override string? ToString()
        {
            switch (_type)
            {
                case Value.ContentEvaluable:
                    return _unionReference.Evaluable.ToString();

                case ValueContent.Boolean:
                    return "<" + (_unionValue.Boolean ? "true" : "false") + ">";

                case ValueContent.Function:
                    return "<" + AsFunction + "()>";

                case ValueContent.Map:
                    var mapBuilder = new StringBuilder();
                    var mapComma = false;
                    var mapKey = 0;

                    mapBuilder.Append('[');

                    foreach (var pair in Fields)
                    {
                        if (mapComma)
                            mapBuilder.Append(", ");
                        else
                            mapComma = true;

                        if (pair.Key.Type == ValueContent.Number &&
                            Math.Abs(pair.Key.AsNumber - mapKey) < double.Epsilon)
                        {
                            ++mapKey;
                        }
                        else
                        {
                            mapBuilder.Append(pair.Key);
                            mapBuilder.Append(": ");
                        }

                        mapBuilder.Append(pair.Value);
                    }

                    mapBuilder.Append(']');

                    return mapBuilder.ToString();

                case ValueContent.Number:
                    return _unionValue.Number.ToString(CultureInfo.InvariantCulture);

                case ValueContent.String:
                    var stringBuilder = new StringBuilder();

                    stringBuilder.Append('"');

                    foreach (var c in _unionReference.String)
                    {
                        if (c == '\\' || c == '"')
                            stringBuilder.Append('\\');

                        stringBuilder.Append(c);
                    }

                    stringBuilder.Append('"');

                    return stringBuilder.ToString();

                case ValueContent.Void:
                    return "<void>";

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}