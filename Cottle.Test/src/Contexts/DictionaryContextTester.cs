using System.Collections.Generic;
using Cottle.Contexts;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class DictionaryContextTester
    {
        [Test]
        public void Find()
        {
            var context = new DictionaryContext(new Dictionary<Value, Value> {{"a", "value"}});

            Assert.That(context["a"], Is.EqualTo(new StringValue("value")));
        }

        [Test]
        public void Miss()
        {
            var context = new DictionaryContext(new Dictionary<Value, Value>());

            Assert.That(context["a"], Is.EqualTo(VoidValue.Instance));
        }
    }
}