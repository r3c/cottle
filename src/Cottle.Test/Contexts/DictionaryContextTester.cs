using System.Collections.Generic;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class DictionaryContextTester
    {
        [Test]
        public void Find()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value> { { "a", "value" } });

            Assert.That(context["a"], Is.EqualTo(Value.FromString("value")));
        }

        [Test]
        public void Miss()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>());

            Assert.That(context["a"], Is.EqualTo(Value.Undefined));
        }
    }
}