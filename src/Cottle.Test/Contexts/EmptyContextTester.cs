using Cottle.Contexts;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class EmptyContextTester
    {
        [Test]
        public void Miss()
        {
            Assert.That(EmptyContext.Instance["a"], Is.EqualTo(Value.Undefined));
        }
    }
}