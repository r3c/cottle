using System.Collections.Generic;
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
            var backend = Context.CreateCustom(new Dictionary<Value, Value>
            {
                ["range"] = new MapValue(i => i * 2, 5)
            });

            var usage = MonitorContextTester.MonitorAndRender("{for i in [0, 1, 2]:{range[i]}}", backend, "024");

            for (var i = 0; i < 3; ++i)
            {
                var rangeUsage = MonitorContextTester.GetChildField(usage, "range", 3, i);

                Assert.That(rangeUsage.Value.Type, Is.EqualTo(ValueContent.Map));

                var fieldUsage = MonitorContextTester.GetChildField(rangeUsage, i, 1, 0);

                Assert.That(fieldUsage.Value.AsNumber, Is.EqualTo(i * 2));
            }
        }

        [Test]
        public static void GroupFieldsUsages()
        {
            const string template = "{set x to parent.child}{x.a}{x.b}{parent.child.c}";

            var root = MonitorContextTester.MonitorAndRender(template, Context.Empty, string.Empty);
            var rootFields = root.GroupFieldUsages();

            Assert.That(rootFields, Does.ContainKey((Value)"parent"));

            var parent = rootFields["parent"];

            Assert.That(parent.Value, Is.EqualTo(VoidValue.Instance));

            var parentFields = parent.GroupFieldUsages();

            Assert.That(parentFields, Does.ContainKey((Value)"child"));

            var child = parentFields["child"];

            Assert.That(child.Value, Is.EqualTo(VoidValue.Instance));

            var childFields = child.GroupFieldUsages();

            Assert.That(childFields, Does.ContainKey((Value)"a"));
            Assert.That(childFields, Does.ContainKey((Value)"b"));
            Assert.That(childFields, Does.ContainKey((Value)"c"));
        }

        [Test]
        public static void MemberAccessDefined()
        {
            var backend = Context.CreateCustom(new Dictionary<Value, Value>
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

            Assert.That(child.Value, Is.EqualTo((Value)"value"));
            Assert.That(child.Fields.Count, Is.EqualTo(0));
        }

        [Test]
        public static void MemberAccessMissing()
        {
            var usage = MonitorContextTester.MonitorAndRender("{parent.child}", Context.Empty, string.Empty);

            Assert.That(usage.Value, Is.EqualTo(VoidValue.Instance));

            var parent = MonitorContextTester.GetChildField(usage, "parent", 1, 0);

            Assert.That(parent.Value, Is.EqualTo(VoidValue.Instance));

            var child = MonitorContextTester.GetChildField(parent, "child", 1, 0);

            Assert.That(child.Value, Is.EqualTo(VoidValue.Instance));
            Assert.That(child.Fields.Count, Is.EqualTo(0));
        }

        [Test]
        public static void MemberAccessMultiple()
        {
            const string template = "{parent.child}{parent.child}";

            var usage = MonitorContextTester.MonitorAndRender(template, Context.Empty, string.Empty);
            var parent0 = MonitorContextTester.GetChildField(usage, "parent", 2, 0);

            Assert.That(parent0.Value, Is.EqualTo(VoidValue.Instance));

            var child0 = MonitorContextTester.GetChildField(parent0, "child", 1, 0);

            Assert.That(child0.Value, Is.EqualTo(VoidValue.Instance));

            var parent1 = MonitorContextTester.GetChildField(usage, "parent", 2, 0);

            Assert.That(parent1.Value, Is.EqualTo(VoidValue.Instance));

            var child1 = MonitorContextTester.GetChildField(parent1, "child", 1, 0);

            Assert.That(child1.Value, Is.EqualTo(VoidValue.Instance));
        }

        [Test]
        public static void MemberChildAccessDefined()
        {
            var backend = Context.CreateCustom(new Dictionary<Value, Value>
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

            Assert.That(subchild.Value, Is.EqualTo((Value)"value"));
            Assert.That(subchild.Fields.Count, Is.EqualTo(0));
        }

        [Test]
        public static void MemberChildAccessMissing()
        {
            var usage = MonitorContextTester.MonitorAndRender("{parent.child.subchild}", Context.Empty, string.Empty);

            Assert.That(usage.Value, Is.EqualTo(VoidValue.Instance));

            var parent = MonitorContextTester.GetChildField(usage, "parent", 1, 0);

            Assert.That(parent.Value, Is.EqualTo(VoidValue.Instance));

            var child = MonitorContextTester.GetChildField(parent, "child", 1, 0);

            Assert.That(child.Value, Is.EqualTo(VoidValue.Instance));

            var subchild = MonitorContextTester.GetChildField(child, "subchild", 1, 0);

            Assert.That(subchild.Value, Is.EqualTo(VoidValue.Instance));
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
            var usage = MonitorContextTester.MonitorAndRender("{scalar}", Context.Empty, string.Empty);
            var scalar = MonitorContextTester.GetChildField(usage, "scalar", 1, 0);

            Assert.That(scalar.Value, Is.EqualTo(VoidValue.Instance));
            Assert.That(scalar.Fields.Count, Is.EqualTo(0));
        }

        private static ISymbolUsage GetChildField(ISymbolUsage parent, Value field, int count, int index)
        {
            Assert.That(parent.Fields.ContainsKey(field), Is.True);
            Assert.That(parent.Fields[field].Count, Is.EqualTo(count));

            return parent.Fields[field][index];
        }

        private static ISymbolUsage MonitorAndRender(string template, IContext backend, string expected)
        {
            var document = new SimpleDocument(template);
            var (context, usage) = Context.CreateMonitor(backend);

            Assert.That(document.Render(context), Is.EqualTo(expected));

            return usage;
        }
    }
}