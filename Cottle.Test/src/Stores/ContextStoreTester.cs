using System.Collections.Generic;
using Cottle.Stores;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Stores
{
    internal class ContextStoreTester
    {
        [Test]
        public void OverwriteWithValue()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value> {{"field", 42}});
            var store = new ContextStore(context) {["field"] = 53};

            Assert.That(store["field"], Is.EqualTo(new NumberValue(53)));
        }

        [Test]
        public void OverwriteWithVoid()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value> {{"field", 42}});
            var store = new ContextStore(context) {["field"] = VoidValue.Instance};

            Assert.That(store["field"], Is.EqualTo(VoidValue.Instance));
        }
    }
}
