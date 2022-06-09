using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Cottle.Evaluables
{
    internal static class ReflectionEvaluable
    {
        private static readonly IReadOnlyDictionary<Type, object> BuiltinConverters = new Dictionary<Type, object>
        {
            { typeof(bool), new Func<bool, Value>(s => s) },
            { typeof(byte), new Func<byte, Value>(s => s) },
            { typeof(char), new Func<char, Value>(s => s) },
            { typeof(double), new Func<double, Value>(s => s) },
            { typeof(decimal), new Func<decimal, Value>(s => s) },
            { typeof(float), new Func<float, Value>(s => s) },
            { typeof(int), new Func<int, Value>(s => s) },
            { typeof(long), new Func<long, Value>(s => s) },
            { typeof(sbyte), new Func<sbyte, Value>(s => s) },
            { typeof(short), new Func<short, Value>(s => s) },
            { typeof(string), new Func<string, Value>(s => s) },
            { typeof(uint), new Func<uint, Value>(s => s) },
            { typeof(ulong), new Func<ulong, Value>(s => (long)s) },
            { typeof(ushort), new Func<ushort, Value>(s => s) }
        };

        private static Dictionary<(BindingFlags, Type), object> CustomReferences = new();

        private static readonly MethodInfo ConverterReferenceConverterGet = Dynamic
            .GetProperty<Func<ConverterReference<object>, Func<object, Value>>>(r => r.Converter)
            .GetMethod!;

        private static readonly ConstructorInfo DictionaryValueValue = Dynamic
            .GetConstructor<Func<Dictionary<Value, Value>>>(() => new Dictionary<Value, Value>());

        private static readonly MethodInfo DictionaryValueValueAdd = Dynamic
            .GetMethod<Action<Dictionary<Value, Value>, Value, Value>>((d, k, v) => d.Add(k, v));

        private static readonly MethodInfo EnumeratorCurrentGet = Dynamic
            .GetProperty<Func<IEnumerator<object>, object>>(e => e.Current)
            .GetMethod!;

        private static readonly MethodInfo EnumerableGetEnumerator = Dynamic
            .GetMethod<Func<IEnumerable<object>, IEnumerator<object>>>(e => e.GetEnumerator());

        private static readonly MethodInfo EnumeratorMoveNext = Dynamic
            .GetMethod<Func<IEnumerator, bool>>(e => e.MoveNext());

        private static readonly MethodInfo Func2Invoke = Dynamic
            .GetMethod<Func<Func<object, object>, object, object>>((c, o) => c.Invoke(o));

        private static readonly MethodInfo KeyValuePairKeyGet = Dynamic
            .GetProperty<Func<KeyValuePair<object, object>, object>>(p => p.Key)
            .GetMethod!;

        private static readonly MethodInfo KeyValuePairValueGet = Dynamic
            .GetProperty<Func<KeyValuePair<object, object>, object>>(p => p.Value)
            .GetMethod!;

        private static readonly ConstructorInfo ListValue = Dynamic
            .GetConstructor<Func<List<Value>>>(() => new List<Value>());

        private static readonly MethodInfo ListValueAdd = Dynamic
            .GetMethod<Action<List<Value>, Value>>((l, v) => l.Add(v));

        private static readonly MethodInfo ReadOnlyListGetItem = Dynamic
            .GetMethod<Func<IReadOnlyList<object>, int, object>>((l, i) => l[i]);

        private static readonly MethodInfo ReflectionEvaluableGetOrCreateConverter = Dynamic
            .GetMethod<Func<BindingFlags, ConverterReference<object>>>((b) => ReflectionEvaluable.GetOrCreateConverter<object>(b))
            .GetGenericMethodDefinition();

        public static Value CreateValue<TSource>(TSource source, BindingFlags bindingFlags)
        {
            return Value.FromEvaluable(new LazyEvaluable(() => ReflectionEvaluable.Resolve(source, bindingFlags)));
        }

        private static Func<TSource, Value> CreateConverter<TSource>(BindingFlags bindingFlags)
        {
            var type = typeof(TSource);
            var interfaces = type.GetInterfaces();

            // Convert dictionary to dictionary-like map value
            var dictionary = interfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));

            if (dictionary is not null)
            {
                var keyType = dictionary.GetGenericArguments()[0];
                var valueType = dictionary.GetGenericArguments()[1];

                return CreateConverterFromDictionary<TSource>(bindingFlags, keyType, valueType);
            }

            // Convert enumerable to array-like map value
            var enumerable = interfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerable is not null)
            {
                var elementType = enumerable.GetGenericArguments()[0];

                return CreateConverterFromEnumerable<TSource>(bindingFlags, elementType);
            }

            // Otherwise browse object fields and properties
            return CreateConverterFromObject<TSource>(bindingFlags);
        }

        private static Func<TSource, Value> CreateConverterFromDictionary<TSource>(BindingFlags bindingFlags, Type keyType, Type valueType)
        {
            var converterType = typeof(object);
            var pairType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
            var sourceType = typeof(TSource);

            var keyConverterReference = ReflectionEvaluableGetOrCreateConverter
                .MakeGenericMethod(keyType)
                .Invoke(null, new object[] { bindingFlags })!;

            var valueConverterReference = ReflectionEvaluableGetOrCreateConverter
                .MakeGenericMethod(valueType)
                .Invoke(null, new object[] { bindingFlags })!;

            var creator = Dynamic.DefineMethod<Func<object, object, TSource, IReadOnlyDictionary<Value, Value>>>();
            var generator = creator.Generator;
            var enumerator = generator.DeclareLocal(typeof(IEnumerator<>).MakeGenericType(pairType));
            var pair = generator.DeclareLocal(pairType);
            var result = generator.DeclareLocal(typeof(Dictionary<Value, Value>));
            var exit = generator.DefineLabel();
            var loop = generator.DefineLabel();

            // enumerator = enumerator.GetEnumerator()
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(EnumerableGetEnumerator, pairType));
            generator.Emit(OpCodes.Stloc, enumerator);

            // result = new Dictionary<Value, Value>()
            generator.Emit(OpCodes.Newobj, DictionaryValueValue);
            generator.Emit(OpCodes.Stloc, result);

            // while (enumerator.MoveNext()) {
            generator.MarkLabel(loop);
            generator.Emit(OpCodes.Ldloc, enumerator);
            generator.Emit(OpCodes.Callvirt, EnumeratorMoveNext);
            generator.Emit(OpCodes.Brfalse, exit);
            generator.Emit(OpCodes.Ldloc, result);

            // key = keyConverterReference.Converter(enumerator.Current.Key)
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(ConverterReferenceConverterGet, keyType));
            generator.Emit(OpCodes.Ldloc, enumerator);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(EnumeratorCurrentGet, pairType));
            generator.Emit(OpCodes.Stloc, pair);
            generator.Emit(OpCodes.Ldloca, pair);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(KeyValuePairKeyGet, keyType, valueType));
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(Func2Invoke, keyType, typeof(Value)));

            // value = valueConverterReference.Converter(enumerator.Current.Value)
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(ConverterReferenceConverterGet, valueType));
            generator.Emit(OpCodes.Ldloc, enumerator);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(EnumeratorCurrentGet, pairType));
            generator.Emit(OpCodes.Stloc, pair);
            generator.Emit(OpCodes.Ldloca, pair);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(KeyValuePairValueGet, keyType, valueType));
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(Func2Invoke, valueType, typeof(Value)));

            // result.Add(key, value)
            generator.Emit(OpCodes.Callvirt, DictionaryValueValueAdd);

            // }
            generator.Emit(OpCodes.Br, loop);

            // return result
            generator.MarkLabel(exit);
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            var method = creator.Create();

            return source =>
            {
                if (source is null)
                    return Value.Undefined;

                var dictionary = method(keyConverterReference, valueConverterReference, source);

                return Value.FromDictionary(dictionary);
            };
        }

        private static Func<TSource, Value> CreateConverterFromEnumerable<TSource>(BindingFlags bindingFlags, Type elementType)
        {
            var converterType = typeof(object);
            var sourceType = typeof(TSource);

            var elementConverterReference = ReflectionEvaluableGetOrCreateConverter
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { bindingFlags })!;

            var creator = Dynamic.DefineMethod<Func<object, TSource, IEnumerable<Value>>>();
            var generator = creator.Generator;
            var enumerator = generator.DeclareLocal(typeof(IEnumerator<>).MakeGenericType(elementType));
            var result = generator.DeclareLocal(typeof(List<Value>));
            var exit = generator.DefineLabel();
            var loop = generator.DefineLabel();

            // enumerator = enumerator.GetEnumerator()
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(EnumerableGetEnumerator, elementType));
            generator.Emit(OpCodes.Stloc, enumerator);

            // result = new List<Value>()
            generator.Emit(OpCodes.Newobj, ListValue);
            generator.Emit(OpCodes.Stloc, result);

            // while (enumerator.MoveNext()) {
            generator.MarkLabel(loop);
            generator.Emit(OpCodes.Ldloc, enumerator);
            generator.Emit(OpCodes.Callvirt, EnumeratorMoveNext);
            generator.Emit(OpCodes.Brfalse, exit);
            generator.Emit(OpCodes.Ldloc, result);

            // element = elementConverterReference.Converter(enumerator.Current)
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(ConverterReferenceConverterGet, elementType));
            generator.Emit(OpCodes.Ldloc, enumerator);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(EnumeratorCurrentGet, elementType));
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(Func2Invoke, elementType, typeof(Value)));

            // result.Add(element)
            generator.Emit(OpCodes.Callvirt, ListValueAdd);

            // }
            generator.Emit(OpCodes.Br, loop);

            // return result
            generator.MarkLabel(exit);
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            var method = creator.Create();

            return source =>
            {
                if (source is null)
                    return Value.Undefined;

                var enumerable = method(elementConverterReference, source);

                return Value.FromEnumerable(enumerable);
            };
        }

        private static Func<TSource, Value> CreateConverterFromObject<TSource>(BindingFlags bindingFlags)
        {
            var converters = new List<object>();
            var convertersType = typeof(IReadOnlyList<object>);
            var readers = new Dictionary<string, Func<IReadOnlyList<object>, TSource, Value>>();
            var sourceType = typeof(TSource);

            foreach (var field in sourceType.GetFields(bindingFlags))
            {
                var fieldConverterReference = ReflectionEvaluableGetOrCreateConverter
                    .MakeGenericMethod(field.FieldType)
                    .Invoke(null, new object[] { bindingFlags })!;

                var creator = Dynamic.DefineMethod<Func<IReadOnlyList<object>, TSource, Value>>();
                var generator = creator.Generator;

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, converters.Count);
                generator.Emit(OpCodes.Callvirt, ReadOnlyListGetItem);
                generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(ConverterReferenceConverterGet, field.FieldType));
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(Func2Invoke, field.FieldType, typeof(Value)));
                generator.Emit(OpCodes.Ret);

                readers[field.Name] = creator.Create();

                converters.Add(fieldConverterReference);
            }

            foreach (var property in sourceType.GetProperties(bindingFlags))
            {
                if (property.GetMethod is null)
                    continue;

                var propertyConverterReference = ReflectionEvaluableGetOrCreateConverter
                    .MakeGenericMethod(property.PropertyType)
                    .Invoke(null, new object[] { bindingFlags })!;

                var creator = Dynamic.DefineMethod<Func<IReadOnlyList<object>, TSource, Value>>();
                var generator = creator.Generator;

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, converters.Count);
                generator.Emit(OpCodes.Callvirt, ReadOnlyListGetItem);
                generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(ConverterReferenceConverterGet, property.PropertyType));

                if (sourceType.IsValueType)
                    generator.Emit(OpCodes.Ldarga, 1);
                else
                    generator.Emit(OpCodes.Ldarg_1);

                generator.Emit(OpCodes.Callvirt, property.GetMethod);
                generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(Func2Invoke, property.PropertyType, typeof(Value)));
                generator.Emit(OpCodes.Ret);

                readers[property.Name] = creator.Create();

                converters.Add(propertyConverterReference);
            }

            return source => source is not null
                ? Value.FromEnumerable(readers.ToDictionary(pair => Value.FromString(pair.Key), pair => Value.FromLazy(() => pair.Value(converters, source))))
                : Value.Undefined;
        }

        private static ConverterReference<TSource> GetOrCreateConverter<TSource>(BindingFlags bindingFlags)
        {
            var type = typeof(TSource);

            // Use converter for known builtin type
            if (ReflectionEvaluable.BuiltinConverters.TryGetValue(type, out var builtinConverter))
                return new ConverterReference<TSource> { Converter = (Func<TSource, Value>)builtinConverter };

            // Return undefined value for other primitive types
            if (type.IsPrimitive)
                return new ConverterReference<TSource>();

            // Use converter for previously built custom type
            if (ReflectionEvaluable.CustomReferences.TryGetValue((bindingFlags, type), out var customReference))
                return (ConverterReference<TSource>)customReference;

            // Otherwise prepare a new converter reference, register it, then build it
            var customReferences = new Dictionary<(BindingFlags, Type), object>(ReflectionEvaluable.CustomReferences);
            var newReference = new ConverterReference<TSource>();

            customReferences[(bindingFlags, type)] = newReference;

            Interlocked.Exchange(ref ReflectionEvaluable.CustomReferences, customReferences);

            // Converter reference is built after being registered to accomodate for recursive types
            newReference.Converter = CreateConverter<TSource>(bindingFlags);

            return newReference;
        }

        private static Value Resolve<TSource>(TSource source, BindingFlags bindingFlags)
        {
            var reference = GetOrCreateConverter<TSource>(bindingFlags);

            return reference.Converter(source);
        }

        private class ConverterReference<TSource>
        {
            public Func<TSource, Value> Converter { get; set; } = _ => Value.Undefined;
        }
    }
}