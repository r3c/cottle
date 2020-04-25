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
            Assert.That(value.AsFunction.Invoke(null, Array.Empty<Value>(), TextWriter.Null),
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
            Assert.That(value.AsFunction.Invoke(null, Array.Empty<Value>(), TextWriter.Null), Is.EqualTo(expected));
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
        public static void FromLazy()
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

        [Test]
        [TestCase(double.NaN)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5.3)]
        public static void FromNumber_Double(double input)
        {
            var value = Value.FromNumber(input);

            Assert.That(value.Type, Is.EqualTo(ValueContent.Number));
            Assert.That(value.AsNumber, Is.EqualTo(input));
        }

        [Test]
        public static void FromNumber_Other()
        {
            var values = new[]
            {
                Value.FromNumber((byte)17),
                Value.FromNumber((double)17),
                Value.FromNumber((float)17),
                Value.FromNumber(17),
                Value.FromNumber((long)17),
                Value.FromNumber((sbyte)17),
                Value.FromNumber((short)17),
                Value.FromNumber((ushort)17),
                Value.FromNumber((uint)17),
                Value.FromNumber((ulong)17)
            };

            foreach (var value in values)
            {
                Assert.That(value.Type, Is.EqualTo(ValueContent.Number));
                Assert.That(value.AsNumber, Is.EqualTo(17));
            }
        }

        [Test]
        public static void FromReflection()
        {
            ValueTester.AssertReadMember(true, Value.True);
            ValueTester.AssertReadMember((byte)4, Value.FromNumber(4));
            ValueTester.AssertReadMember((sbyte)-5, Value.FromNumber(-5));
            ValueTester.AssertReadMember((short)-9, Value.FromNumber(-9));
            ValueTester.AssertReadMember((ushort)8, Value.FromNumber(8));
            ValueTester.AssertReadMember(42, Value.FromNumber(42));
            ValueTester.AssertReadMember(42u, Value.FromNumber(42u));
            ValueTester.AssertReadMember(17L, Value.FromNumber(17L));
            ValueTester.AssertReadMember(24LU, Value.FromNumber(24L));
            ValueTester.AssertReadMember(1f, Value.FromNumber(1f));
            ValueTester.AssertReadMember(3d, Value.FromNumber(3d));
            ValueTester.AssertReadMember('x', Value.FromString("x"));
            ValueTester.AssertReadMember("abc", Value.FromString("abc"));
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

        private static void AssertReadMember<T>(T reference, Value expected)
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

        private class FieldContainer<T>
        {
            // ReSharper disable once NotAccessedField.Local
            public T Field;
        }

        private class PropertyContainer<T>
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public T Property { get; set; }
        }
    }
}