using System.IO;
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
            var context = Context.CreateBuiltin(Context.Empty);
            var function = context["add"].AsFunction;
            var result = function.Execute(new Value[] { 1, 2 }, new SimpleStore(), new StringWriter());

            Assert.That(result, Is.EqualTo((Value)3));
        }

        [Test]
        public void GetMissingBuiltin()
        {
            var context = Context.CreateBuiltin(Context.Empty);

            Assert.That(context["dummy"], Is.EqualTo(VoidValue.Instance));
        }
    }
}