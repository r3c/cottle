using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Cottle.Evaluables
{
    internal static class ReflectionEvaluable
    {
        private static readonly Dictionary<Type, Func<object, Value>> Converters =
            new Dictionary<Type, Func<object, Value>>
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

        private static readonly Dictionary<Type, IReadOnlyList<MemberReader>> Readers =
            new Dictionary<Type, IReadOnlyList<MemberReader>>();

        public static Value CreateValue(object source, BindingFlags bindingFlags)
        {
            return Value.FromEvaluable(new LazyEvaluable(() => ReflectionEvaluable.Resolve(source, bindingFlags)));
        }

        private static Value Resolve(object source, BindingFlags bindingFlags)
        {
            IReadOnlyList<MemberReader> readers;

            var type = source.GetType();

            // Use converter for known primitive types
            if (ReflectionEvaluable.Converters.TryGetValue(type, out var converter))
                return converter(source);

            // Return undefined value for other primitive types
            if (type.IsPrimitive)
                return Value.Undefined;

            // Convert elements to array if source object is enumerable
            if (source is IEnumerable enumerable)
            {
                var elements = new List<Value>();

                foreach (var element in enumerable)
                    elements.Add(ReflectionEvaluable.CreateValue(element, bindingFlags));

                return elements;
            }

            // Otherwise, browse object fields and properties
            var fields = new Dictionary<Value, Value>();

            lock (ReflectionEvaluable.Readers)
            {
                if (!ReflectionEvaluable.Readers.TryGetValue(type, out readers))
                {
                    var memberReaders = new List<MemberReader>();

                    foreach (var field in type.GetFields(bindingFlags))
                        memberReaders.Add(new MemberReader(field, bindingFlags));

                    foreach (var property in type.GetProperties(bindingFlags))
                        memberReaders.Add(new MemberReader(property, bindingFlags));

                    ReflectionEvaluable.Readers[type] = memberReaders;

                    readers = memberReaders;
                }
            }

            foreach (var extractor in readers)
                fields.Add(extractor.Name, extractor.Extract(source));

            return fields;
        }

        private readonly struct MemberReader
        {
            public string Name { get; }

            private readonly BindingFlags _binding;

            private readonly Func<object, object> _extractor;

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

            public Value Extract(object source)
            {
                var value = _extractor(source);

                return value != null ? ReflectionEvaluable.CreateValue(value, _binding) : Value.Undefined;
            }
        }
    }
}