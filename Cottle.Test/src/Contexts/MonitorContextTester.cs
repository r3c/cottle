using System.Collections.Generic;
using Cottle.Contexts;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public static class MonitorContextTester
    {
        [Test]
        public static void DirectAccessExistingComplex()
        {
            var backend = new DictionaryContext(new Dictionary<Value, Value>
            {
                {
                    "parent", new Dictionary<Value, Value>
                    {
                        {"child", "value"}
                    }
                }
            });

            var context = new MonitorContext(backend);

            // Trigger store access
            Assert.IsTrue(context["parent"].Fields.TryGet("child", out var child));
            Assert.AreEqual("value", child.AsString);

            // Assert usage
            Assert.AreEqual(1, context.Usage.Fields.Count);
            Assert.IsTrue(context.Usage.Fields.ContainsKey("parent"));
            Assert.AreEqual(1, context.Usage.Fields["parent"].Count);

            var firstAccessToParent = context.Usage.Fields["parent"][0];

            Assert.AreEqual(ValueContent.Map, firstAccessToParent.Value.Type);
            Assert.AreEqual(1, firstAccessToParent.Fields.Count);
            Assert.IsTrue(firstAccessToParent.Fields.ContainsKey("child"));
            Assert.AreEqual(1, firstAccessToParent.Fields["child"].Count);

            var firstAccessToChild = firstAccessToParent.Fields["child"][0];

            Assert.AreEqual(ValueContent.String, firstAccessToChild.Value.Type);
            Assert.AreEqual("value", firstAccessToChild.Value.AsString);
            Assert.AreEqual(0, firstAccessToChild.Fields.Count);
        }

        [Test]
        public static void DirectAccessExistingScalar()
        {
            var context = new MonitorContext(Context.CreateCustom(new Dictionary<Value, Value> {{"a", 17}}));

            Assert.That(context["a"], Is.EqualTo(new NumberValue(17)));

            Assert.That(context.Usage.Fields.Count, Is.EqualTo(1));
            Assert.That(context.Usage.Fields["a"].Count, Is.EqualTo(1));
            Assert.That(context.Usage.Fields["a"][0].Fields, Is.Empty);
            Assert.That(context.Usage.Fields["a"][0].Value, Is.EqualTo(new NumberValue(17)));
        }

        [Test]
        public static void DirectAccessMissingComplex()
        {
            var context = new MonitorContext(EmptyContext.Instance);

            // Trigger store access
            Assert.IsFalse(context["parent"].Fields.TryGet("child", out _));

            // Assert usage
            Assert.AreEqual(1, context.Usage.Fields.Count);
            Assert.IsTrue(context.Usage.Fields.ContainsKey("parent"));
            Assert.AreEqual(1, context.Usage.Fields["parent"].Count);

            var firstAccessToParent = context.Usage.Fields["parent"][0];

            Assert.AreEqual(ValueContent.Void, firstAccessToParent.Value.Type);
            Assert.AreEqual(1, firstAccessToParent.Fields.Count);
            Assert.IsTrue(firstAccessToParent.Fields.ContainsKey("child"));
            Assert.AreEqual(1, firstAccessToParent.Fields["child"].Count);

            var firstAccessToChild = firstAccessToParent.Fields["child"][0];

            Assert.AreEqual(ValueContent.Void, firstAccessToChild.Value.Type);
            Assert.AreEqual(0, firstAccessToChild.Fields.Count);
        }

        [Test]
        public static void DirectAccessMissingScalar()
        {
            var context = new MonitorContext(EmptyContext.Instance);

            // Trigger store access
            Assert.AreEqual(VoidValue.Instance, context["scalar"]);

            // Assert usage
            Assert.AreEqual(1, context.Usage.Fields.Count);
            Assert.IsTrue(context.Usage.Fields.ContainsKey("scalar"));
            Assert.AreEqual(1, context.Usage.Fields["scalar"].Count);

            var firstAccessToScalar = context.Usage.Fields["scalar"][0];

            Assert.AreEqual(ValueContent.Void, firstAccessToScalar.Value.Type);
            Assert.AreEqual(0, firstAccessToScalar.Fields.Count);
        }

        [Test]
        public static void EnumerateGenerator()
        {
            var backend = new DictionaryContext(new Dictionary<Value, Value>
            {
                {"range", new MapValue(i => i * 2, 5)}
            });

            var monitor = new MonitorContext(backend);
            var index = 0;

            foreach (var pair in monitor["range"].Fields)
            {
                Assert.AreEqual(ValueContent.Number, pair.Value.Type);
                Assert.AreEqual(index * 2, pair.Value.AsNumber);

                if (index++ == 3)
                    break;
            }

            // Assert usage
            Assert.AreEqual(1, monitor.Usage.Fields.Count);
            Assert.IsTrue(monitor.Usage.Fields.ContainsKey("range"));
            Assert.AreEqual(1, monitor.Usage.Fields["range"].Count);

            var firstAccessToRange = monitor.Usage.Fields["range"][0];

            Assert.AreEqual(ValueContent.Map, firstAccessToRange.Value.Type);
            Assert.AreEqual(4, firstAccessToRange.Fields.Count);

            for (var i = 0; i < 4; ++i)
            {
                Assert.AreEqual(1, firstAccessToRange.Fields[i].Count);

                var accessToSubscript = firstAccessToRange.Fields[i][0];

                Assert.AreEqual(ValueContent.Number, accessToSubscript.Value.Type);
                Assert.AreEqual(i * 2, accessToSubscript.Value.AsNumber);
                Assert.AreEqual(0, accessToSubscript.Value.Fields.Count);
            }
        }
    }
}