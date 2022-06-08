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

        private static Dictionary<(BindingFlags, Type), object> CustomConverters = new();

        private static readonly MethodInfo EnumeratorCurrentGet = Reflection
            .GetProperty<Func<IEnumerator<object>, object>>(e => e.Current)
            .GetMethod!;

        private static readonly MethodInfo EnumerableGetEnumerator = Reflection
            .GetMethod<Func<IEnumerable<object>, IEnumerator<object>>>(e => e.GetEnumerator());

        private static readonly MethodInfo EnumeratorMoveNext = Reflection
            .GetMethod<Func<IEnumerator, bool>>(e => e.MoveNext());

        private static readonly MethodInfo Func2Invoke = Reflection
            .GetMethod<Func<Func<object, object>, object, object>>((c, o) => c.Invoke(o));

        private static readonly ConstructorInfo ListValue = Reflection
            .GetConstructor<Func<List<Value>>>(() => new List<Value>());

        private static readonly MethodInfo ListValueAdd = Reflection
            .GetMethod<Action<List<Value>, Value>>((l, v) => l.Add(v));

        private static readonly MethodInfo ReadOnlyListGetItem = Reflection
            .GetMethod<Func<IReadOnlyList<object>, int, object>>((l, i) => l[i]);

        private static readonly MethodInfo ReflectionEvaluableGetOrCreateConverter = Reflection
            .GetMethod<Func<BindingFlags, Func<object, Value>>>((b) => ReflectionEvaluable.GetOrCreateConverter<object>(b))
            .GetGenericMethodDefinition();

        public static Value CreateValue<TSource>(TSource source, BindingFlags bindingFlags)
        {
            return Value.FromEvaluable(new LazyEvaluable(() => ReflectionEvaluable.Resolve(source, bindingFlags)));
        }

        private static Func<TSource, Value> CreateConverter<TSource>(BindingFlags bindingFlags)
        {
            var type = typeof(TSource);
            var interfaces = type.GetInterfaces();

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

        private static Func<TSource, Value> CreateConverterFromEnumerable<TSource>(BindingFlags bindingFlags, Type elementType)
        {
            var converterType = typeof(object);
            var sourceType = typeof(TSource);
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(IEnumerable<Value>), new[] { converterType, sourceType }, typeof(ReflectionEvaluable).Module, true);
            var generator = dynamicMethod.GetILGenerator();
            var enumerator = generator.DeclareLocal(typeof(IEnumerator<>).MakeGenericType(elementType));
            var result = generator.DeclareLocal(typeof(List<Value>));
            var exit = generator.DefineLabel();
            var loop = generator.DefineLabel();

            var elementConverter = ReflectionEvaluableGetOrCreateConverter
                .MakeGenericMethod(elementType)
                .Invoke(null, new object[] { bindingFlags })!;

            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Callvirt, Reflection.ChangeGenericDeclaringType(EnumerableGetEnumerator, elementType));
            generator.Emit(OpCodes.Stloc, enumerator);
            generator.Emit(OpCodes.Newobj, ListValue);
            generator.Emit(OpCodes.Stloc, result);
            generator.MarkLabel(loop);
            generator.Emit(OpCodes.Ldloc, enumerator);
            generator.Emit(OpCodes.Callvirt, EnumeratorMoveNext);
            generator.Emit(OpCodes.Brfalse, exit);
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldloc, enumerator);
            generator.Emit(OpCodes.Callvirt, Reflection.ChangeGenericDeclaringType(EnumeratorCurrentGet, elementType));
            generator.Emit(OpCodes.Callvirt, Reflection.ChangeGenericDeclaringType(Func2Invoke, elementType, typeof(Value)));
            generator.Emit(OpCodes.Callvirt, ListValueAdd);
            generator.Emit(OpCodes.Br, loop);
            generator.MarkLabel(exit);
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);

            var method = (Func<object, TSource, IEnumerable<Value>>)dynamicMethod.CreateDelegate(typeof(Func<object, TSource, IEnumerable<Value>>));

            return source =>
            {
                if (source is null)
                    return Value.Undefined;

                var enumerable = method(elementConverter, source);

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
                var fieldConverter = ReflectionEvaluableGetOrCreateConverter
                    .MakeGenericMethod(field.FieldType)
                    .Invoke(null, new object[] { bindingFlags })!;

                var dynamicMethod = new DynamicMethod(string.Empty, typeof(Value), new[] { convertersType, sourceType }, typeof(ReflectionEvaluable).Module, true);
                var generator = dynamicMethod.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, converters.Count);
                generator.Emit(OpCodes.Callvirt, ReadOnlyListGetItem);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Callvirt, Reflection.ChangeGenericDeclaringType(Func2Invoke, field.FieldType, typeof(Value)));
                generator.Emit(OpCodes.Ret);

                var fieldReader = (Func<IReadOnlyList<object>, TSource, Value>)dynamicMethod.CreateDelegate(typeof(Func<IReadOnlyList<object>, TSource, Value>));

                readers[field.Name] = fieldReader;

                converters.Add(fieldConverter);
            }

            foreach (var property in sourceType.GetProperties(bindingFlags))
            {
                if (property.GetMethod is null)
                    continue;

                var propertyConverter = ReflectionEvaluableGetOrCreateConverter
                    .MakeGenericMethod(property.PropertyType)
                    .Invoke(null, new object[] { bindingFlags })!;

                var dynamicMethod = new DynamicMethod(string.Empty, typeof(Value), new[] { convertersType, sourceType }, typeof(ReflectionEvaluable).Module, true);
                var generator = dynamicMethod.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, converters.Count);
                generator.Emit(OpCodes.Callvirt, ReadOnlyListGetItem);

                if (sourceType.IsValueType)
                    generator.Emit(OpCodes.Ldarga, 1);
                else
                    generator.Emit(OpCodes.Ldarg_1);

                generator.Emit(OpCodes.Callvirt, property.GetMethod);
                generator.Emit(OpCodes.Callvirt, Reflection.ChangeGenericDeclaringType(Func2Invoke, property.PropertyType, typeof(Value)));
                generator.Emit(OpCodes.Ret);

                var propertyReader = (Func<IReadOnlyList<object>, TSource, Value>)dynamicMethod.CreateDelegate(typeof(Func<IReadOnlyList<object>, TSource, Value>));

                readers[property.Name] = propertyReader;

                converters.Add(propertyConverter);
            }

            return source => source is not null
                ? Value.FromEnumerable(readers.ToDictionary(pair => Value.FromString(pair.Key), pair => pair.Value(converters, source)))
                : Value.Undefined;
        }

        private static Func<TSource, Value> GetOrCreateConverter<TSource>(BindingFlags bindingFlags)
        {
            var type = typeof(TSource);

            // Use converter for known builtin type
            if (ReflectionEvaluable.BuiltinConverters.TryGetValue(type, out var builtinConverter))
                return (Func<TSource, Value>)builtinConverter;

            // Return undefined value for other primitive types
            if (type.IsPrimitive)
                return _ => Value.Undefined;

            // Use converter for previously built custom type
            if (ReflectionEvaluable.CustomConverters.TryGetValue((bindingFlags, type), out var customConverter))
                return (Func<TSource, Value>)customConverter;

            var customConverters = new Dictionary<(BindingFlags, Type), object>(ReflectionEvaluable.CustomConverters);
            var typeConverter = CreateConverter<TSource>(bindingFlags);

            customConverters[(bindingFlags, type)] = typeConverter;

            Interlocked.Exchange(ref ReflectionEvaluable.CustomConverters, customConverters);

            return typeConverter;
        }

        private static Value Resolve<TSource>(TSource source, BindingFlags bindingFlags)
        {
            var converter = GetOrCreateConverter<TSource>(bindingFlags);

            return converter(source);
        }
    }
}