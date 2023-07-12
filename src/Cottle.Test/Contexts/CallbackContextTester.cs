using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class CallbackContextTester
    {
        [Test]
        public void GetDefined()
        {
            var context = Context.CreateCustom(symbol => symbol == "defined" ? (Value)3 : Value.Undefined);

            Assert.That(context["defined"], Is.EqualTo((Value)3));
        }

        [Test]
        public void GetMissing()
        {
            var context = Context.CreateCustom(symbol => symbol == "defined" ? (Value)3 : Value.Undefined);

            Assert.That(context["missing"], Is.EqualTo(Value.Undefined));
        }

        [Test]
        public void GetNull()
        {
            var context = Context.CreateCustom(_ => default);

            Assert.That(context["something"], Is.EqualTo(Value.Undefined));
        }
    }
}