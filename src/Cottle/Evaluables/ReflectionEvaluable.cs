using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using Cottle.Exceptions;

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
            { typeof(ushort), new Func<ushort, Value>(s => s) },

            { typeof(bool?), new Func<bool?, Value>(s => s ?? Value.Undefined) },
            { typeof(byte?), new Func<byte?, Value>(s => s ?? Value.Undefined) },
            { typeof(char?), new Func<char?, Value>(s => s ?? Value.Undefined) },
            { typeof(double?), new Func<double?, Value>(s => s ?? Value.Undefined) },
            { typeof(decimal?), new Func<decimal?, Value>(s => s ?? Value.Undefined) },
            { typeof(float?), new Func<float?, Value>(s => s ?? Value.Undefined) },
            { typeof(int?), new Func<int?, Value>(s => s ?? Value.Undefined) },
            { typeof(long?), new Func<long?, Value>(s => s ?? Value.Undefined) },
            { typeof(sbyte?), new Func<sbyte?, Value>(s => s ?? Value.Undefined) },
            { typeof(short?), new Func<short?, Value>(s => s ?? Value.Undefined) },
            { typeof(uint?), new Func<uint?, Value>(s => s ?? Value.Undefined) },
            { typeof(ulong?), new Func<ulong?, Value>(s => s.HasValue ? (long)s.Value : Value.Undefined) },
            { typeof(ushort?), new Func<ushort?, Value>(s => s ?? Value.Undefined) },
        };

        private static Dictionary<(BindingFlags, Type), object> _customReferences = new();

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

        private static readonly MethodInfo ValueFromReflection = Dynamic
            .GetMethod<Func<object, BindingFlags, Value>>((o, f) => Value.FromReflection(o, f))
            .GetGenericMethodDefinition();

        public static IEvaluable CreateEvaluable<TSource>(TSource source, BindingFlags bindingFlags)
        {
            return new LazyEvaluable(() => ReflectionEvaluable.Resolve(source, bindingFlags));
        }

        private static Func<TSource, Value> CreateConverter<TSource>(BindingFlags bindingFlags)
        {
            var enumerable = typeof(TSource)
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            // Convert object fields and properties if not enumerable
            if (enumerable is null)
                return CreateConverterFromObject<TSource>(bindingFlags);

            var elementType = enumerable.GetGenericArguments()[0];

            // Convert dictionary-like map value if element type is KeyValuePair<TKey, TValue>
            if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var keyType = elementType.GetGenericArguments()[0];
                var valueType = elementType.GetGenericArguments()[1];

                return CreateConverterFromDictionary<TSource>(bindingFlags, keyType, valueType);
            }

            // Otherwise convert to array-like map value
            return CreateConverterFromEnumerable<TSource>(bindingFlags, elementType);
        }

        private static Func<TSource, Value> CreateConverterFromDictionary<TSource>(BindingFlags bindingFlags,
            Type keyType, Type valueType)
        {
            var pairType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);

            var creator = Dynamic.DeclareMethod<Func<TSource, BindingFlags, IReadOnlyDictionary<Value, Value>>>();
            var generator = creator.Generator;
            var enumerator = generator.DeclareLocal(typeof(IEnumerator<>).MakeGenericType(pairType));
            var pair = generator.DeclareLocal(pairType);
            var result = generator.DeclareLocal(typeof(Dictionary<Value, Value>));
            var exit = generator.DefineLabel();
            var loop = generator.DefineLabel();

            // enumerator = enumerator.GetEnumerator()
            generator.Emit(OpCodes.Ldarg_0);
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

            // pair = enumerator.Current
            generator.Emit(OpCodes.Ldloc, enumerator);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(EnumeratorCurrentGet, pairType));
            generator.Emit(OpCodes.Stloc, pair);

            // key = Value.FromReflection(pair.Key, _bindingFlags)
            generator.Emit(OpCodes.Ldloca, pair);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(KeyValuePairKeyGet, keyType,
                valueType));
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, CreateRecursiveConstruction(keyType));

            // value = Value.FromReflection(pair.Value, _bindingFlags)
            generator.Emit(OpCodes.Ldloca, pair);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(KeyValuePairValueGet, keyType,
                valueType));
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, CreateRecursiveConstruction(valueType));

            // result.Add(key, value)
            generator.Emit(OpCodes.Callvirt, DictionaryValueValueAdd);

            // }
            generator.Emit(OpCodes.Br, loop);

            // return result
            generator.MarkLabel(exit);
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            var method = creator.Create();

            return source => source is not null
                ? Value.FromDictionary(method(source, bindingFlags))
                : Value.Undefined;
        }

        private static Func<TSource, Value> CreateConverterFromEnumerable<TSource>(BindingFlags bindingFlags,
            Type elementType)
        {
            var creator = Dynamic.DeclareMethod<Func<TSource, BindingFlags, IEnumerable<Value>>>();
            var generator = creator.Generator;
            var enumerator = generator.DeclareLocal(typeof(IEnumerator<>).MakeGenericType(elementType));
            var result = generator.DeclareLocal(typeof(List<Value>));
            var exit = generator.DefineLabel();
            var loop = generator.DefineLabel();

            // enumerator = enumerator.GetEnumerator()
            generator.Emit(OpCodes.Ldarg_0);
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

            // element = Value.FromReflection(enumerator.Current, _bindingFlags)
            generator.Emit(OpCodes.Ldloc, enumerator);
            generator.Emit(OpCodes.Callvirt, Dynamic.ChangeGenericDeclaringType(EnumeratorCurrentGet, elementType));
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, CreateRecursiveConstruction(elementType));

            // result.Add(element)
            generator.Emit(OpCodes.Callvirt, ListValueAdd);

            // }
            generator.Emit(OpCodes.Br, loop);

            // return result
            generator.MarkLabel(exit);
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            var method = creator.Create();

            return source => source is not null
                ? Value.FromEnumerable(method(source, bindingFlags))
                : Value.Undefined;
        }

        private static Func<TSource, Value> CreateConverterFromObject<TSource>(BindingFlags bindingFlags)
        {
            var readers = new Dictionary<string, Func<TSource, BindingFlags, Value>>();
            var sourceType = typeof(TSource);

            foreach (var field in sourceType.GetFields(bindingFlags))
            {
                if (field.IsDefined(typeof(CompilerGeneratedAttribute), false))
                    continue;

                var creator = Dynamic.DeclareMethod<Func<TSource, BindingFlags, Value>>();
                var generator = creator.Generator;

                if (field.IsStatic)
                    generator.Emit(OpCodes.Ldsfld, field);
                else
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, field);
                }

                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, CreateRecursiveConstruction(field.FieldType));
                generator.Emit(OpCodes.Ret);

                readers[field.Name] = creator.Create();
            }

            foreach (var property in sourceType.GetProperties(bindingFlags))
            {
                if (property.IsDefined(typeof(CompilerGeneratedAttribute), false) ||
                    property.GetMethod is null ||
                    property.GetMethod.GetParameters().Length > 0)
                    continue;

                var method = property.GetMethod;

                if (!method.IsPublic && !bindingFlags.HasFlag(BindingFlags.NonPublic))
                    continue;

                var creator = Dynamic.DeclareMethod<Func<TSource, BindingFlags, Value>>();
                var generator = creator.Generator;

                if (method.IsStatic)
                    generator.Emit(OpCodes.Call, method);
                else
                {
                    if (sourceType.IsValueType)
                        generator.Emit(OpCodes.Ldarga, 0);
                    else
                        generator.Emit(OpCodes.Ldarg_0);

                    generator.Emit(OpCodes.Callvirt, method);
                }

                generator.Emit(OpCodes.Ldarg_1);

                try
                {
                    generator.Emit(OpCodes.Call, ValueFromReflection.MakeGenericMethod(property.PropertyType));
                }
                catch (ArgumentException exception)
                {
                    throw new UnconvertiblePropertyException(property, exception);
                }

                generator.Emit(OpCodes.Ret);

                readers[property.Name] = creator.Create();
            }

            return source => source is not null
                ? Value.FromEnumerable(readers.ToDictionary(
                    pair => Value.FromString(pair.Key),
                    pair => pair.Value(source, bindingFlags)))
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
            if (_customReferences.TryGetValue((bindingFlags, type), out var customReference))
                return (ConverterReference<TSource>)customReference;

            // Otherwise prepare a new converter reference, register it, then build it
            var customReferences = new Dictionary<(BindingFlags, Type), object>(_customReferences);
            var newReference = new ConverterReference<TSource>();

            customReferences[(bindingFlags, type)] = newReference;

            Interlocked.Exchange(ref _customReferences, customReferences);

            // Converter reference is built after being registered to accomodate for recursive types
            newReference.Converter = CreateConverter<TSource>(bindingFlags);

            return newReference;
        }

        private static MethodInfo CreateRecursiveConstruction(Type type)
        {
            try
            {
                return ValueFromReflection.MakeGenericMethod(type);
            }
            catch (ArgumentException exception)
            {
                throw new ReflectionTypeException(type, exception);
            }
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