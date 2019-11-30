using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class CallbackContextTester
    {
        [Test]
        public void GetDefined()
        {
            var context = Context.CreateCustom(symbol => symbol == "defined" ? (Value)3 : VoidValue.Instance);

            Assert.That(context["defined"], Is.EqualTo((Value)3));
        }

        [Test]
        public void GetMissing()
        {
            var context = Context.CreateCustom(symbol => symbol == "defined" ? (Value)3 : VoidValue.Instance);

            Assert.That(context["missing"], Is.EqualTo(VoidValue.Instance));
        }

        [Test]
        public void GetNull()
        {
            var context = Context.CreateCustom(symbol => null);

            Assert.That(context["something"], Is.EqualTo(VoidValue.Instance));
        }
    }
}