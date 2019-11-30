using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Values
{
    public static class ReflectionValueTester
    {
        [Test]
        public static void ReadMember()
        {
            ReflectionValueTester.ReadMember(true, new BooleanValue(true));
            ReflectionValueTester.ReadMember((byte)4, new NumberValue((byte)4));
            ReflectionValueTester.ReadMember((sbyte)-5, new NumberValue((sbyte)-5));
            ReflectionValueTester.ReadMember((short)-9, new NumberValue((short)-9));
            ReflectionValueTester.ReadMember((ushort)8, new NumberValue(8));
            ReflectionValueTester.ReadMember(42, new NumberValue(42));
            ReflectionValueTester.ReadMember(42u, new NumberValue(42u));
            ReflectionValueTester.ReadMember(17L, new NumberValue(17L));
            ReflectionValueTester.ReadMember(24LU, new NumberValue(24L));
            ReflectionValueTester.ReadMember(1f, new NumberValue(1f));
            ReflectionValueTester.ReadMember(3d, new NumberValue(3d));
            ReflectionValueTester.ReadMember(5m, new NumberValue(5m));
            ReflectionValueTester.ReadMember('x', new StringValue("x"));
            ReflectionValueTester.ReadMember("abc", new StringValue("abc"));
        }

        private static void ReadMember<T>(T reference, Value expected)
        {
            var container = new Container<T>();
            var value = new ReflectionValue(container);

            container.Field = reference;
            container.Property = reference;

            Assert.That(value.Fields.TryGet("Field", out var result), Is.True, "reflection value must have a 'Field' key");
            Assert.That(result, Is.EqualTo(expected), "reflection value should be able to read field of type {0}",
                typeof(T));
            Assert.That(value.Fields.TryGet("Property", out result), Is.True,
                "reflection value must have a 'Property' key");
            Assert.That(result, Is.EqualTo(expected), "reflection value should be able to read property of type {0}",
                typeof(T));
        }

        private class Container<T>
        {
            public T Field;

            public T Property { get; set; }
        }
    }
}