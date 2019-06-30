using System.Collections.Generic;
using Cottle.Contexts;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class MonitorContextTester
    {
        [Test]
        public void CountComplexSymbol()
        {
            var context = new MonitorContext(Context.CreateCustom(new Dictionary<Value, Value>
                {{"a", new Dictionary<Value, Value> {{"b", 42}}}}));

            Assert.That(context["a"].Fields.TryGet("b", out var value), Is.True);
            Assert.That(value, Is.EqualTo(new NumberValue(42)));

            Assert.That(context.Symbols.Count, Is.EqualTo(1));
            Assert.That(context.Symbols["a"].Count, Is.EqualTo(1));
            Assert.That(context.Symbols["a"][0].Value.Type, Is.EqualTo(ValueContent.Map));
            Assert.That(context.Symbols["a"][0].Fields.Count, Is.EqualTo(1));
            Assert.That(context.Symbols["a"][0].Fields["b"].Count, Is.EqualTo(1));
            Assert.That(context.Symbols["a"][0].Fields["b"][0].Fields, Is.Empty);
            Assert.That(context.Symbols["a"][0].Fields["b"][0].Value, Is.EqualTo(new NumberValue(42)));
        }

        [Test]
        public void CountScalarSymbol()
        {
            var context = new MonitorContext(Context.CreateCustom(new Dictionary<Value, Value> {{"a", 17}}));

            Assert.That(context["a"], Is.EqualTo(new NumberValue(17)));

            Assert.That(context.Symbols.Count, Is.EqualTo(1));
            Assert.That(context.Symbols["a"].Count, Is.EqualTo(1));
            Assert.That(context.Symbols["a"][0].Fields, Is.Empty);
            Assert.That(context.Symbols["a"][0].Value, Is.EqualTo(new NumberValue(17)));
        }
    }
}