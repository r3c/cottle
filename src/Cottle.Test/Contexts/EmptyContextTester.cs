using Cottle.Contexts;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public class EmptyContextTester
    {
        [Test]
        public void Miss()
        {
            Assert.That(EmptyContext.Instance["a"], Is.EqualTo(VoidValue.Instance));
        }
    }
}