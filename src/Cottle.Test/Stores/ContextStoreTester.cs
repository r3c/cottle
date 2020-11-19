using System.Collections.Generic;
using Cottle.Stores;
using NUnit.Framework;

namespace Cottle.Test.Stores
{
    internal class ContextStoreTester
    {
        [Test]
        public void AccessorOverwriteWithValue()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value> { { "field", 42 } });
            var store = new ContextStore(context) { ["field"] = 53 };

            Assert.That(store["field"], Is.EqualTo(Value.FromNumber(53)));
        }

        [Test]
        public void AccessorOverwriteWithVoid()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value> { { "field", 42 } });
            var store = new ContextStore(context) { ["field"] = Value.Undefined };

            Assert.That(store["field"], Is.EqualTo(Value.Undefined));
        }

        [Test]
        public void TryGetWithUndefined()
        {
            var store = new ContextStore(Context.Empty);

            Assert.That(store.TryGet("field", out _), Is.False);
        }

        [Test]
        public void TryGetWithValue()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value> { ["field"] = 1 });
            var store = new ContextStore(context);

            Assert.That(store.TryGet("field", out _), Is.True);
        }

        [Test]
        public void TryGetWithVoid()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value> { ["field"] = Value.Undefined });
            var store = new ContextStore(context);

            Assert.That(store.TryGet("field", out _), Is.False);
        }
    }
}