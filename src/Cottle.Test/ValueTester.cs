using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cottle.Maps;
using Moq;
using NUnit.Framework;

namespace Cottle.Test
{
    public static class ValueTester
    {
        [Test]
        public static void EmptyMap()
        {
            Assert.That(Value.EmptyMap.Type, Is.EqualTo(ValueContent.Map));
            Assert.That(Value.EmptyMap.Fields.Count, Is.EqualTo(0));
        }

        [Test]
        public static void EmptyString()
        {
            Assert.That(Value.EmptyString.Type, Is.EqualTo(ValueContent.String));
            Assert.That(Value.EmptyString.AsString, Is.EqualTo(string.Empty));
        }

        [Test]
        public static void Equals_ShouldCompareBooleans()
        {
            Assert.That(Value.False, Is.EqualTo(Value.False));
            Assert.That(Value.False, Is.Not.EqualTo(Value.True));
            Assert.That(Value.True, Is.EqualTo(Value.True));
            Assert.That(Value.False, Is.Not.EqualTo(Value.Zero));
        }

        [Test]
        public static void Equals_ShouldCompareMaps()
        {
            var a = new KeyValuePair<Value, Value>("A", 1);
            var b = new KeyValuePair<Value, Value>("B", 2);
            var c = new KeyValuePair<Value, Value>("C", 3);
            var d = new KeyValuePair<Value, Value>("D", 4);

            Assert.That(Value.FromEnumerable(new[] { a, b, c }), Is.EqualTo(Value.FromEnumerable(new[] { a, b, c })));
            Assert.That(Value.FromEnumerable(new[] { a, b, c }), Is.Not.EqualTo(Value.FromEnumerable(new[] { a, b, d })));
            Assert.That(Value.FromEnumerable(new[] { a, b, c }), Is.Not.EqualTo(Value.False));
        }

        [Test]
        public static void Equals_ShouldCompareNumbers()
        {
            Assert.That(Value.Zero, Is.EqualTo(Value.Zero));
            Assert.That(Value.Zero, Is.Not.EqualTo(Value.FromNumber(1)));
            Assert.That(Value.Zero, Is.Not.EqualTo(Value.EmptyString));
        }

        [Test]
        public static void Equals_ShouldCompareStrings()
        {
            Assert.That(Value.FromString("A"), Is.EqualTo(Value.FromString("A")));
            Assert.That(Value.FromString("A"), Is.Not.EqualTo(Value.FromString("B")));
            Assert.That(Value.FromString("A"), Is.Not.EqualTo(Value.False));
        }

        [Test]
        public static void Equals_ShouldCompareVoid()
        {
            Assert.That(Value.Undefined, Is.EqualTo(Value.Undefined));
            Assert.That(Value.Undefined, Is.Not.EqualTo(Value.False));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public static void FromBoolean(bool input)
        {
            var value = Value.FromBoolean(input);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Boolean));
            Assert.That(value.AsBoolean, Is.EqualTo(input));
        }

        [Test]
        [TestCase('a')]
        [TestCase('©')]
        public static void FromCharacter(char input)
        {
            var value = Value.FromCharacter(input);

            Assert.That(value.Type, Is.EqualTo(ValueContent.String));
            Assert.That(value.AsString[0], Is.EqualTo(input));
        }

        [Test]
        public static void FromDictionary()
        {
            var elements = new Dictionary<Value, Value> { ["A"] = 17, [42] = "B" };
            var value = Value.FromDictionary(elements);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Map));
            Assert.That(new List<KeyValuePair<Value, Value>>(value.Fields), Is.EqualTo(elements));
        }

        [Test]
        public static void FromEnumerable_KeyValue()
        {
            var elements = new[] { new KeyValuePair<Value, Value>("A", 17), new KeyValuePair<Value, Value>(42, "B") };
            var value = Value.FromEnumerable(elements);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Map));
            Assert.That(new List<KeyValuePair<Value, Value>>(value.Fields), Is.EqualTo(elements));
        }

        [Test]
        public static void FromEnumerable_Value()
        {
            var elements = new Value[] { 4, 8, 15, 16, 23, 42 };
            var expected = elements.Select((element, index) => new KeyValuePair<Value, Value>(index, element)).ToList();
            var value = Value.FromEnumerable(elements);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Map));
            Assert.That(new List<KeyValuePair<Value, Value>>(value.Fields), Is.EqualTo(expected));
        }

        [Test]
        public static void FromEvaluable()
        {
            var evaluable = new Mock<IEvaluable>();
            var value = Value.FromEvaluable(evaluable.Object);

            evaluable.Setup(e => e.Type).Returns(ValueContent.Boolean);
            evaluable.Setup(e => e.AsBoolean).Returns(true);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Boolean));
            Assert.That(value.AsBoolean, Is.True);

            evaluable.Setup(e => e.Type).Returns(ValueContent.Number);
            evaluable.Setup(e => e.AsNumber).Returns(17);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Number));
            Assert.That(value.AsNumber, Is.EqualTo(17d));

            evaluable.Setup(e => e.Type).Returns(ValueContent.Function);
            evaluable.Setup(e => e.AsFunction).Returns(Function.CreatePure((s, a) => 42));

            Assert.That(value.Type, Is.EqualTo(ValueContent.Function));
            Assert.That(value.AsFunction.Invoke(new(), Array.Empty<Value>(), TextWriter.Null),
                Is.EqualTo(Value.FromNumber(42)));

            evaluable.Setup(e => e.Type).Returns(ValueContent.String);
            evaluable.Setup(e => e.AsString).Returns("test");

            Assert.That(value.Type, Is.EqualTo(ValueContent.String));
            Assert.That(value.AsString, Is.EqualTo("test"));

            evaluable.Setup(e => e.Type).Returns(ValueContent.Map);
            evaluable.Setup(e => e.Fields).Returns(new ArrayMap(new Value[] { 1, 3, 7 }));

            Assert.That(value.Type, Is.EqualTo(ValueContent.Map));
            Assert.That(value.Fields.Count, Is.EqualTo(3));
            Assert.That(value.Fields[0], Is.EqualTo(Value.FromNumber(1)));
            Assert.That(value.Fields[1], Is.EqualTo(Value.FromNumber(3)));
            Assert.That(value.Fields[2], Is.EqualTo(Value.FromNumber(7)));

            evaluable.Setup(e => e.Type).Returns(ValueContent.Void);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Void));
        }

        [Test]
        public static void FromFunction()
        {
            var expected = Value.FromNumber(59);
            var value = Value.FromFunction(Function.CreatePure((s, a) => expected));

            Assert.That(value.Type, Is.EqualTo(ValueContent.Function));
            Assert.That(value.AsFunction.IsPure, Is.True);
            Assert.That(value.AsFunction.Invoke(new(), Array.Empty<Value>(), TextWriter.Null), Is.EqualTo(expected));
        }

        [Test]
        public static void FromGenerator()
        {
            var value = Value.FromGenerator(i => i * 3, 5);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Map));
            Assert.That(value.Fields.Count, Is.EqualTo(5));
            Assert.That(value.Fields[0], Is.EqualTo(Value.Zero));
            Assert.That(value.Fields[4], Is.EqualTo(Value.FromNumber(12)));
            Assert.That(value.Fields[5], Is.EqualTo(Value.Undefined));
        }

        [Test]
        [TestCaseSource(nameof(Values))]
        public static void FromLazy_ShouldCompare(Value resolved)
        {
            Assert.That(Value.FromLazy(() => resolved), Is.EqualTo(resolved));
            Assert.That(Value.FromLazy(() => Value.FromLazy(() => resolved)), Is.EqualTo(resolved));
        }

        [Test]
        public static void FromLazy_ShouldResolve()
        {
            var resolved = false;
            var resolver = new Func<Value>(() =>
            {
                resolved = true;

                return 13;
            });

            var value = Value.FromLazy(resolver);

            Assert.That(resolved, Is.False);
            Assert.That(value.Type, Is.EqualTo(ValueContent.Number));
            Assert.That(value.AsNumber, Is.EqualTo(13d));
            Assert.That(resolved, Is.True);
        }

        [Test]
        public static void FromMap()
        {
            var expected = Value.FromString("field");
            var index = Value.FromString("index");
            var map = new Mock<IMap>();
            var value = Value.FromMap(map.Object);

            map.Setup(m => m[index]).Returns(expected);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Map));
            Assert.That(value.Fields[index], Is.EqualTo(expected));
        }

        private static readonly IReadOnlyList<TestCaseData> FromNumber_Input = new[]
        {
            new TestCaseData(Value.FromNumber(double.NaN), double.NaN),
            new TestCaseData(Value.FromNumber(0d), 0),
            new TestCaseData(Value.FromNumber(1d), 1),
            new TestCaseData(Value.FromNumber(5.3d), 5.3),
            new TestCaseData(Value.FromNumber((byte)17), 17),
            new TestCaseData(Value.FromNumber(17d), 17),
            new TestCaseData(Value.FromNumber(17f), 17),
            new TestCaseData(Value.FromNumber(17), 17),
            new TestCaseData(Value.FromNumber(17L), 17),
            new TestCaseData(Value.FromNumber((sbyte)17), 17),
            new TestCaseData(Value.FromNumber((short)17), 17),
            new TestCaseData(Value.FromNumber((ushort)17), 17),
            new TestCaseData(Value.FromNumber((uint)17), 17),
            new TestCaseData(Value.FromNumber(17UL), 17)
        };

        [Test]
        [TestCaseSource(nameof(FromNumber_Input))]
        public static void FromNumber(Value value, double expected)
        {
            Assert.That(value.Type, Is.EqualTo(ValueContent.Number));
            Assert.That(value.AsNumber, Is.EqualTo(expected));
        }

        private static readonly IReadOnlyList<TestCaseData> FromReflection_Input = new[]
        {
            new TestCaseData(true, Value.True),
            new TestCaseData((byte)4, Value.FromNumber(4)),
            new TestCaseData((sbyte)-5, Value.FromNumber(-5)),
            new TestCaseData((short)-9, Value.FromNumber(-9)),
            new TestCaseData((ushort)8, Value.FromNumber(8)),
            new TestCaseData(42, Value.FromNumber(42)),
            new TestCaseData(42u, Value.FromNumber(42u)),
            new TestCaseData(17L, Value.FromNumber(17L)),
            new TestCaseData(24LU, Value.FromNumber(24L)),
            new TestCaseData(1f, Value.FromNumber(1f)),
            new TestCaseData(3d, Value.FromNumber(3d)),
            new TestCaseData('x', Value.FromString("x")),
            new TestCaseData("abc", Value.FromString("abc"))
        };

        [Test]
        [TestCaseSource(nameof(FromReflection_Input))]
        public static void FromReflection<T>(T reference, Value expected)
        {
            // Read from field member
            var fieldContainer = new FieldContainer<T>();
            var fieldValue = Value.FromReflection(fieldContainer, BindingFlags.Instance | BindingFlags.Public);

            fieldContainer.Field = reference;

            Assert.That(fieldValue.Fields.TryGet("Field", out var result), Is.True, "value has no 'Field' key");
            Assert.That(result, Is.EqualTo(expected), "value should be able to read field of type {0}", typeof(T));

            // Read from property member
            var propertyContainer = new PropertyContainer<T>();
            var propertyValue = Value.FromReflection(propertyContainer, BindingFlags.Instance | BindingFlags.Public);

            propertyContainer.Property = reference;

            Assert.That(propertyValue.Fields.TryGet("Property", out result), Is.True, "value has no 'Property' key");
            Assert.That(result, Is.EqualTo(expected), "value should be able to read property of type {0}", typeof(T));
        }

        [Test]
        [TestCase("")]
        [TestCase("Hello!")]
        public static void FromString_NotNull(string input)
        {
            var value = Value.FromString(input);

            Assert.That(value.Type, Is.EqualTo(ValueContent.String));
            Assert.That(value.AsString, Is.EqualTo(input));
        }

        [Test]
        public static void FromString_Null()
        {
            var value = Value.FromString(null);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Void));
        }

        [Test]
        public static void False()
        {
            Assert.That(Value.False.Type, Is.EqualTo(ValueContent.Boolean));
            Assert.That(Value.False.AsBoolean, Is.False);
        }

        [Test]
        public static void True()
        {
            Assert.That(Value.True.Type, Is.EqualTo(ValueContent.Boolean));
            Assert.That(Value.True.AsBoolean, Is.True);
        }

        [Test]
        public static void Undefined()
        {
            Assert.That(Value.Undefined, Is.EqualTo(default(Value)));
            Assert.That(Value.Undefined.Type, Is.EqualTo(ValueContent.Void));
        }

        [Test]
        public static void Zero()
        {
            Assert.That(Value.Zero.Type, Is.EqualTo(ValueContent.Number));
            Assert.That(Value.Zero.AsNumber, Is.EqualTo(0));
        }

        private class FieldContainer<T>
        {
            // ReSharper disable once NotAccessedField.Local
            public T Field = default!;
        }

        private class PropertyContainer<T>
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public T Property { get; set; } = default!;
        }

        private static readonly IReadOnlyList<TestCaseData> Values = new[]
        {
            new TestCaseData(Value.EmptyMap),
            new TestCaseData(Value.EmptyString),
            new TestCaseData(Value.False),
            new TestCaseData(Value.True),
            new TestCaseData(Value.Undefined),
            new TestCaseData(Value.Zero),
            new TestCaseData(Value.FromCharacter('a')),
            new TestCaseData(Value.FromDictionary(new Dictionary<Value, Value> { ["A"] = 1, ["B"] = 2 })),
            new TestCaseData(Value.FromEnumerable(new Value[] {1, "A", true})),
            new TestCaseData(Value.FromNumber(42)),
            new TestCaseData(Value.FromString("abc"))
        };
    }
}