using System.Collections.Generic;
using Cottle.Contexts;
using NUnit.Framework;

namespace Cottle.Test.Contexts
{
    public static class SpyContextTester
    {
        [Test]
        public static void SpyVariable_ShouldSpyAfterRenderScalar()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                ["scalar"] = "test"
            });

            var document = Document.CreateDefault("{scalar}").DocumentOrThrow;
            var spyContext = SpyAndRender(document, context);
            var scalar = spyContext.SpyVariable("scalar");

            Assert.That(scalar.Value, Is.EqualTo((Value)"test"));
        }

        [Test]
        public static void SpyVariable_ShouldSpyAfterRenderUndefined()
        {
            var document = Document.CreateDefault("{scalar}").DocumentOrThrow;
            var spyContext = SpyAndRender(document, Context.Empty);
            var scalar = spyContext.SpyVariable("scalar");

            Assert.That(scalar.Value, Is.EqualTo(Value.Undefined));
        }

        [Test]
        public static void SpyVariable_ShouldSpyBeforeRender()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                ["scalar"] = "test"
            });

            var document = Document.CreateDefault("{scalar}").DocumentOrThrow;
            var spyContext = Context.CreateSpy(context);
            var scalar = spyContext.SpyVariable("scalar");

            Assert.That(document.Render(spyContext), Is.EqualTo("test"));
            Assert.That(scalar.Value, Is.EqualTo((Value)"test"));
        }

        [Test]
        public static void SpyVariable_ShouldSpyFieldAndSubField()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                ["parent"] = new Dictionary<Value, Value>
                {
                    ["child"] = new Dictionary<Value, Value>
                    {
                        ["subchild"] = "value"
                    }
                }
            });

            var document = Document.CreateDefault("{parent.child.subchild}").DocumentOrThrow;
            var spyContext = SpyAndRender(document, context);
            var parent = spyContext.SpyVariable("parent");

            Assert.That(parent.Value.Type, Is.EqualTo(ValueContent.Map));

            var child = parent.SpyField("child");

            Assert.That(child.Value.Type, Is.EqualTo(ValueContent.Map));

            var subchild = child.SpyField("subchild");

            Assert.That(subchild.Value, Is.EqualTo((Value)"value"));
        }

        [Test]
        public static void SpyVariable_ShouldSpyFieldScalar()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                ["parent"] = new Dictionary<Value, Value>
                {
                    ["child"] = "value"
                }
            });

            var document = Document.CreateDefault("{parent.child}").DocumentOrThrow;
            var spyContext = SpyAndRender(document, context);
            var parent = spyContext.SpyVariable("parent");

            Assert.That(parent.Value.Type, Is.EqualTo(ValueContent.Map));

            var child = parent.SpyField("child");

            Assert.That(child.Value, Is.EqualTo((Value)"value"));
        }

        [Test]
        public static void SpyVariable_ShouldSpyFieldUndefined()
        {
            var document = Document.CreateDefault("{parent.child}").DocumentOrThrow;
            var spyContext = SpyAndRender(document, Context.Empty);
            var parent = spyContext.SpyVariable("parent");

            Assert.That(parent.Value, Is.EqualTo(Value.Undefined));

            var child = parent.SpyField("child");

            Assert.That(child.Value, Is.EqualTo(Value.Undefined));
        }

        [Test]
        public static void SpyVariables_ShouldSpyAliasedSubFields()
        {
            const string template = "{set x to parent.child}{x.a}{x.b}{parent.child.c}";

            var document = Document.CreateDefault(template).DocumentOrThrow;
            var spyContext = SpyAndRender(document, Context.Empty);
            var variables = spyContext.SpyVariables();

            Assert.That(variables, Does.ContainKey((Value)"parent"));

            var parent = variables["parent"];

            Assert.That(parent.Value, Is.EqualTo(Value.Undefined));

            var parentFields = parent.SpyFields();

            Assert.That(parentFields, Does.ContainKey((Value)"child"));

            var child = parentFields["child"];

            Assert.That(child.Value, Is.EqualTo(Value.Undefined));

            var childFields = child.SpyFields();

            Assert.That(childFields, Does.ContainKey((Value)"a"));
            Assert.That(childFields, Does.ContainKey((Value)"b"));
            Assert.That(childFields, Does.ContainKey((Value)"c"));
        }

        [Test]
        public static void SpyVariables_ShouldSpyGeneratedValues()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                ["range"] = Value.FromGenerator(i => i * 2, 5)
            });

            var document = Document.CreateDefault("{for i in [0, 1, 2]:{range[i]}}").DocumentOrThrow;
            var spyContext = SpyAndRender(document, context);
            var range = spyContext.SpyVariable("range");

            Assert.That(range.Value.Type, Is.EqualTo(ValueContent.Map));

            for (var i = 0; i < 3; ++i)
            {
                var field = range.SpyField(i);

                Assert.That(field.Value.AsNumber, Is.EqualTo(i * 2));
            }
        }

        [Test]
        public static void SpyVariables_ShouldSpyMultipleValues()
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value> { { "a", 17 } });

            var document = Document.CreateDefault("{a}{b}").DocumentOrThrow;
            var spyContext = SpyAndRender(document, context);
            var variables = spyContext.SpyVariables();

            Assert.That(variables.Count, Is.EqualTo(2));
            Assert.That(variables, Does.ContainKey((Value)"a"));
            Assert.That(variables["a"].Value, Is.EqualTo(Value.FromNumber(17)));
            Assert.That(variables, Does.ContainKey((Value)"b"));
            Assert.That(variables["b"].Value, Is.EqualTo(Value.Undefined));
        }

        private static ISpyContext SpyAndRender(IDocument document, IContext context)
        {
            var spyContext = Context.CreateSpy(context);

            Assert.That(document.Render(spyContext), Is.EqualTo(document.Render(context)), $"{nameof(SpyContext)} must not affect rendering");

            return spyContext;
        }
    }
}