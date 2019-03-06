using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Values
{
    public static class ReflectionValueTester
    {
        [Test]
        public static void ReadMember()
        {
            ReadMember(true, new BooleanValue(true));
            ReadMember((byte)4, new NumberValue((byte)4));
            ReadMember((sbyte)-5, new NumberValue((sbyte)-5));
            ReadMember((short)-9, new NumberValue((short)-9));
            ReadMember((ushort)8, new NumberValue(8));
            ReadMember(42, new NumberValue(42));
            ReadMember(42u, new NumberValue(42u));
            ReadMember(17L, new NumberValue(17L));
            ReadMember(24LU, new NumberValue(24L));
            ReadMember(1f, new NumberValue(1f));
            ReadMember(3d, new NumberValue(3d));
            ReadMember(5m, new NumberValue(5m));
            ReadMember('x', new StringValue("x"));
            ReadMember("abc", new StringValue("abc"));
        }

        private static void ReadMember<T>(T reference, Value expected)
        {
            var container = new Container<T>();
            var value = new ReflectionValue(container);

            container.Field = reference;
            container.Property = reference;

            Assert.IsTrue(value.Fields.TryGet("Field", out var result), "reflection value must have a 'Field' key");
            Assert.AreEqual(expected, result, "reflection value should be able to read field of type {0}", typeof(T));
            Assert.IsTrue(value.Fields.TryGet("Property", out result), "reflection value must have a 'Property' key");
            Assert.AreEqual(expected, result, "reflection value should be able to read property of type {0}",
                typeof(T));
        }

        private class Container<T>
        {
            public T Field;

            public T Property { get; set; }
        }
    }
}