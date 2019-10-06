using System.Collections.Generic;
using Cottle.Contexts;
using Cottle.Contexts.Monitor;
using Cottle.Documents;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public static class MonitorContextTester
    {
        [Test]
        public static void GeneratorAccessDefined()
        {
            var backend = new DictionaryContext(new Dictionary<Value, Value>
            {
                ["range"] = new MapValue(i => i * 2, 5)
            });

            var usage = MonitorContextTester.MonitorAndRender("{for i in [0, 1, 2]:{range[i]}}", backend, "024");

            for (var i = 0; i < 3; ++i)
            {
                var rangeUsage = MonitorContextTester.GetChildField(usage, "range", 3, i);

                Assert.That(rangeUsage.Value.Type, Is.EqualTo(ValueContent.Map));
                Assert.That(rangeUsage.Fields, Does.ContainKey((Value)i));
                Assert.That(rangeUsage.Fields[i].Count, Is.EqualTo(1));
                Assert.That(rangeUsage.Fields[i][0].Value.AsNumber, Is.EqualTo(i * 2));
            }
        }

        [Test]
        public static void MemberAccessDefined()
        {
            var backend = new DictionaryContext(new Dictionary<Value, Value>
            {
                ["parent"] = new Dictionary<Value, Value>
                {
                    ["child"] = "value"
                }
            });

            var usage = MonitorContextTester.MonitorAndRender("{parent.child}", backend, "value");
            var parent = MonitorContextTester.GetChildField(usage, "parent", 1, 0);

            Assert.That(parent.Value.Type, Is.EqualTo(ValueContent.Map));

            var child = MonitorContextTester.GetChildField(parent, "child", 1, 0);

            Assert.That(child.Value.Type, Is.EqualTo(ValueContent.String));
            Assert.That(child.Value.AsString, Is.EqualTo("value"));
            Assert.That(child.Fields.Count, Is.EqualTo(0));
        }

        [Test]
        public static void MemberAccessMissing()
        {
            var usage = MonitorContextTester.MonitorAndRender("{parent.child}", EmptyContext.Instance, string.Empty);
            var parent = MonitorContextTester.GetChildField(usage, "parent", 1, 0);

            Assert.That(parent.Value.Type, Is.EqualTo(ValueContent.Void));

            var child = MonitorContextTester.GetChildField(parent, "child", 1, 0);

            Assert.That(child.Value.Type, Is.EqualTo(ValueContent.Void));
            Assert.That(child.Fields.Count, Is.EqualTo(0));
        }

        [Test]
        public static void MemberChildAccessDefined()
        {
            var backend = new DictionaryContext(new Dictionary<Value, Value>
            {
                ["parent"] = new Dictionary<Value, Value>
                {
                    ["child"] = new Dictionary<Value, Value>
                    {
                        ["subchild"] = "value"
                    }
                }
            });

            var usage = MonitorContextTester.MonitorAndRender("{parent.child.subchild}", backend, "value");
            var parent = MonitorContextTester.GetChildField(usage, "parent", 1, 0);

            Assert.That(parent.Value.Type, Is.EqualTo(ValueContent.Map));

            var child = MonitorContextTester.GetChildField(parent, "child", 1, 0);

            Assert.That(child.Value.Type, Is.EqualTo(ValueContent.Map));

            var subchild = MonitorContextTester.GetChildField(child, "subchild", 1, 0);

            Assert.That(subchild.Value.AsString, Is.EqualTo("value"));
            Assert.That(subchild.Fields.Count, Is.EqualTo(0));
        }

        [Test]
        public static void MemberChildAccessMissing()
        {
            var usage = MonitorContextTester.MonitorAndRender("{parent.child.subchild}", Context.Empty, string.Empty);
            var parent = MonitorContextTester.GetChildField(usage, "parent", 1, 0);

            Assert.That(parent.Value.Type, Is.EqualTo(ValueContent.Void));

            var child = MonitorContextTester.GetChildField(parent, "child", 1, 0);

            Assert.That(child.Value.Type, Is.EqualTo(ValueContent.Void));

            var subchild = MonitorContextTester.GetChildField(child, "subchild", 1, 0);

            Assert.That(subchild.Value.Type, Is.EqualTo(ValueContent.Void));
        }

        [Test]
        public static void ScalarAccessDefined()
        {
            var backend = Context.CreateCustom(new Dictionary<Value, Value> { { "a", 17 } });
            var usage = MonitorContextTester.MonitorAndRender("{a}", backend, "17");

            Assert.That(usage.Fields.Count, Is.EqualTo(1));
            Assert.That(usage.Fields["a"].Count, Is.EqualTo(1));
            Assert.That(usage.Fields["a"][0].Fields, Is.Empty);
            Assert.That(usage.Fields["a"][0].Value, Is.EqualTo(new NumberValue(17)));
        }

        [Test]
        public static void ScalarAccessMissing()
        {
            var usage = MonitorContextTester.MonitorAndRender("{scalar}", EmptyContext.Instance, string.Empty);
            var scalar = MonitorContextTester.GetChildField(usage, "scalar", 1, 0);

            Assert.That(scalar.Value.Type, Is.EqualTo(ValueContent.Void));
            Assert.That(scalar.Fields.Count, Is.EqualTo(0));
        }

        private static ISymbolUsage GetChildField(ISymbolUsage parent, string field, int count, int index)
        {
            Assert.That(parent.Fields.ContainsKey(field), Is.True);
            Assert.That(parent.Fields[field].Count, Is.EqualTo(count));

            return parent.Fields[field][index];
        }

        private static ISymbolUsage MonitorAndRender(string template, IContext backend, string expected)
        {
            var document = new SimpleDocument(template);
            var monitor = new MonitorContext(backend);

            Assert.That(document.Render(monitor), Is.EqualTo(expected));

            return monitor.Usage;
        }
    }
}