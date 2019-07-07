using System.Collections.Generic;
using Cottle.Contexts;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class CascadeContextTester
    {
        [Test]
        public void FindFromFallbackWithEmptyPrimary()
        {
            var primary = Context.CreateEmpty();
            var fallback = Context.CreateCustom(new Dictionary<Value, Value> { { "a", "fallback" } });
            var context = new CascadeContext(primary, fallback);

            Assert.That(context["a"], Is.EqualTo(new StringValue("fallback")));
        }

        [Test]
        public void FindFromFallbackWithVoidPrimary()
        {
            var primary = Context.CreateCustom(new Dictionary<Value, Value> { { "a", VoidValue.Instance } });
            var fallback = Context.CreateCustom(new Dictionary<Value, Value> { { "a", "fallback" } });
            var context = new CascadeContext(primary, fallback);

            Assert.That(context["a"], Is.EqualTo(new StringValue("fallback")));
        }

        [Test]
        public void FindFromPrimary()
        {
            var primary = Context.CreateCustom(new Dictionary<Value, Value> { { "a", "primary" } });
            var fallback = Context.CreateCustom(new Dictionary<Value, Value> { { "a", "fallback" } });
            var context = new CascadeContext(primary, fallback);

            Assert.That(context["a"], Is.EqualTo(new StringValue("primary")));
        }

        [Test]
        public void Miss()
        {
            var primary = Context.CreateCustom(new Dictionary<Value, Value> { { "b", "primary" } });
            var fallback = Context.CreateCustom(new Dictionary<Value, Value> { { "b", "fallback" } });
            var context = new CascadeContext(primary, fallback);

            Assert.That(context["a"], Is.EqualTo(VoidValue.Instance));
        }
    }
}