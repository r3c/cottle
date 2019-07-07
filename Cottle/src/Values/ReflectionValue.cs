using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cottle.Values
{
    public sealed class ReflectionValue : ResolveValue
    {
        #region Methods

        protected override Value Resolve()
        {
            List<MemberReader> reader;

            var type = _source.GetType();

            // Use converter for known primitive types
            if (ReflectionValue.Converters.TryGetValue(type, out var converter))
                return converter(_source);

            // Return undefined value for other primitive types
            if (type.IsPrimitive)
                return VoidValue.Instance;

            // Convert elements to array if source object is enumerable
            if (_source is IEnumerable enumerable)
            {
                var elements = new List<Value>();

                foreach (var element in enumerable)
                    elements.Add(new ReflectionValue(element));

                return elements;
            }

            // Otherwise, browse object fields and properties
            var fields = new Dictionary<Value, Value>();

            lock (ReflectionValue.Readers)
            {
                if (!ReflectionValue.Readers.TryGetValue(type, out reader))
                {
                    reader = new List<MemberReader>();

                    foreach (var field in type.GetFields(_binding))
                        reader.Add(new MemberReader(field, _binding));

                    foreach (var property in type.GetProperties(_binding))
                        reader.Add(new MemberReader(property, _binding));

                    ReflectionValue.Readers[type] = reader;
                }
            }

            foreach (var extractor in reader)
                fields.Add(extractor.Name, extractor.Extract(_source));

            return fields;
        }

        #endregion

        #region Attributes / Instance

        private readonly BindingFlags _binding;

        private readonly object _source;

        #endregion

        #region Attributes / Static

        private static readonly Dictionary<Type, ValueConverter> Converters = new Dictionary<Type, ValueConverter>
        {
            { typeof(bool), s => (bool)s },
            { typeof(byte), s => (byte)s },
            { typeof(char), s => (char)s },
            { typeof(double), s => (double)s },
            { typeof(decimal), s => (decimal)s },
            { typeof(float), s => (float)s },
            { typeof(int), s => (int)s },
            { typeof(long), s => (long)s },
            { typeof(sbyte), s => (sbyte)s },
            { typeof(short), s => (short)s },
            { typeof(string), s => (string)s },
            { typeof(uint), s => (uint)s },
            { typeof(ulong), s => (long)(ulong)s },
            { typeof(ushort), s => (ushort)s }
        };

        private static readonly Dictionary<Type, List<MemberReader>> Readers =
            new Dictionary<Type, List<MemberReader>>();

        #endregion

        #region Constructors

        public ReflectionValue(object source, BindingFlags binding)
        {
            _binding = binding;
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public ReflectionValue(object source) :
            this(source, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
        }

        #endregion

        #region Types

        private struct MemberReader
        {
            #region Properties

            public string Name { get; }

            #endregion

            #region Attributes

            private readonly BindingFlags _binding;

            private readonly Func<object, object> _extractor;

            #endregion

            #region Constructors

            public MemberReader(FieldInfo field, BindingFlags binding)
            {
                _binding = binding;
                _extractor = field.GetValue;

                Name = field.Name;
            }

            public MemberReader(PropertyInfo property, BindingFlags binding)
            {
                var method = property.GetGetMethod(true);

                _binding = binding;
                _extractor = s => method.Invoke(s, null);

                Name = property.Name;
            }

            #endregion

            #region Methods

            public Value Extract(object source)
            {
                var value = _extractor(source);

                if (value != null)
                    return new ReflectionValue(value, _binding);

                return VoidValue.Instance;
            }

            #endregion
        }

        private delegate Value ValueConverter(object source);

        #endregion
    }
}