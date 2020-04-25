using System.IO;
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
            var result = function.Invoke(Context.Empty, new Value[] { 1, 2 }, new StringWriter());

            Assert.That(result, Is.EqualTo((Value)3));
        }

        [Test]
        public void GetMissingBuiltin()
        {
            var context = Context.CreateBuiltin(Context.Empty);

            Assert.That(context["dummy"], Is.EqualTo(Value.Undefined));
        }
    }
}