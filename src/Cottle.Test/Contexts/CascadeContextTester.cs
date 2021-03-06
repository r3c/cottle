using System.Collections.Generic;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class CascadeContextTester
    {
        [Test]
        public void FindFromFallbackWithEmptyPrimary()
        {
            var primary = Context.Empty;
            var fallback = Context.CreateCustom(new Dictionary<Value, Value> { { "a", "fallback" } });
            var context = Context.CreateCascade(primary, fallback);

            Assert.That(context["a"], Is.EqualTo(Value.FromString("fallback")));
        }

        [Test]
        public void FindFromFallbackWithVoidPrimary()
        {
            var primary = Context.CreateCustom(new Dictionary<Value, Value> { { "a", Value.Undefined } });
            var fallback = Context.CreateCustom(new Dictionary<Value, Value> { { "a", "fallback" } });
            var context = Context.CreateCascade(primary, fallback);

            Assert.That(context["a"], Is.EqualTo(Value.FromString("fallback")));
        }

        [Test]
        public void FindFromPrimary()
        {
            var primary = Context.CreateCustom(new Dictionary<Value, Value> { { "a", "primary" } });
            var fallback = Context.CreateCustom(new Dictionary<Value, Value> { { "a", "fallback" } });
            var context = Context.CreateCascade(primary, fallback);

            Assert.That(context["a"], Is.EqualTo(Value.FromString("primary")));
        }

        [Test]
        public void Miss()
        {
            var primary = Context.CreateCustom(new Dictionary<Value, Value> { { "b", "primary" } });
            var fallback = Context.CreateCustom(new Dictionary<Value, Value> { { "b", "fallback" } });
            var context = Context.CreateCascade(primary, fallback);

            Assert.That(context["a"], Is.EqualTo(Value.Undefined));
        }
    }
}