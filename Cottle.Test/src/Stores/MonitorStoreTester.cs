using System.Collections.Generic;
using Cottle.Stores;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Stores
{
    public static class MonitorStoreTester
    {
        [Test]
        public static void DirectAccessExistingComplex()
        {
            var backend = new SimpleStore();
            var monitor = new MonitorStore(backend);

            backend["parent"] = new Dictionary<Value, Value>
            {
                {"child", "value"}
            };

            // Trigger store access
            Assert.IsTrue(monitor["parent"].Fields.TryGet("child", out var child));
            Assert.AreEqual("value", child.AsString);

            // Assert usage
            Assert.AreEqual(1, monitor.Symbols.Count);
            Assert.IsTrue(monitor.Symbols.ContainsKey("parent"));
            Assert.AreEqual(1, monitor.Symbols["parent"].Count);

            var firstAccessToParent = monitor.Symbols["parent"][0];

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
            var backend = new SimpleStore();
            var monitor = new MonitorStore(backend);

            // Trigger store access
            backend["scalar"] = 42;

            Assert.IsTrue(monitor.TryGet("scalar", out _));

            backend["scalar"] = 41;

            Assert.AreEqual(41, monitor["scalar"].AsNumber);

            // Assert usage
            Assert.AreEqual(1, monitor.Symbols.Count);
            Assert.IsTrue(monitor.Symbols.ContainsKey("scalar"));
            Assert.AreEqual(2, monitor.Symbols["scalar"].Count);

            var firstAccessToScalar = monitor.Symbols["scalar"][0];

            Assert.AreEqual(ValueContent.Number, firstAccessToScalar.Value.Type);
            Assert.AreEqual(42, firstAccessToScalar.Value.AsNumber);
            Assert.AreEqual(0, firstAccessToScalar.Fields.Count);

            var secondAccessToScalar = monitor.Symbols["scalar"][1];

            Assert.AreEqual(ValueContent.Number, firstAccessToScalar.Value.Type);
            Assert.AreEqual(41, secondAccessToScalar.Value.AsNumber);
            Assert.AreEqual(0, secondAccessToScalar.Value.Fields.Count);
        }

        [Test]
        public static void DirectAccessMissingComplex()
        {
            var monitor = new MonitorStore(new SimpleStore());

            // Trigger store access
            Assert.IsFalse(monitor["parent"].Fields.TryGet("child", out _));

            // Assert usage
            Assert.AreEqual(1, monitor.Symbols.Count);
            Assert.IsTrue(monitor.Symbols.ContainsKey("parent"));
            Assert.AreEqual(1, monitor.Symbols["parent"].Count);

            var firstAccessToParent = monitor.Symbols["parent"][0];

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
            var monitor = new MonitorStore(new SimpleStore());

            // Trigger store access
            Assert.AreEqual(VoidValue.Instance, monitor["scalar"]);

            // Assert usage
            Assert.AreEqual(1, monitor.Symbols.Count);
            Assert.IsTrue(monitor.Symbols.ContainsKey("scalar"));
            Assert.AreEqual(1, monitor.Symbols["scalar"].Count);

            var firstAccessToScalar = monitor.Symbols["scalar"][0];

            Assert.AreEqual(ValueContent.Void, firstAccessToScalar.Value.Type);
            Assert.AreEqual(0, firstAccessToScalar.Fields.Count);
        }

        [Test]
        public static void EnumerateGenerator()
        {
            var backend = new SimpleStore();
            var monitor = new MonitorStore(backend);

            backend["range"] = new MapValue(i => i * 2, 5);

            var index = 0;

            foreach (var pair in monitor["range"].Fields)
            {
                Assert.AreEqual(ValueContent.Number, pair.Value.Type);
                Assert.AreEqual(index * 2, pair.Value.AsNumber);

                if (index++ == 3)
                    break;
            }

            // Assert usage
            Assert.AreEqual(1, monitor.Symbols.Count);
            Assert.IsTrue(monitor.Symbols.ContainsKey("range"));
            Assert.AreEqual(1, monitor.Symbols["range"].Count);

            var firstAccessToRange = monitor.Symbols["range"][0];

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
