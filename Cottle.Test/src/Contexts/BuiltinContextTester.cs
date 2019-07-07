using System.IO;
using Cottle.Contexts;
using Cottle.Stores;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class BuiltinContextTester
    {
        [Test]
        public void GetExistingBuiltin()
        {
            var function = BuiltinContext.Instance["add"].AsFunction;
            var result = function.Execute(new Value[] { 1, 2 }, new SimpleStore(), new StringWriter());

            Assert.That(result, Is.EqualTo(new NumberValue(3)));
        }

        [Test]
        public void GetMissingBuiltin()
        {
            Assert.That(BuiltinContext.Instance["dummy"], Is.EqualTo(VoidValue.Instance));
        }
    }
}