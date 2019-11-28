using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test
{
    public class FunctionTester
    {
        private static readonly Value Return = 42;
        private const string Output = "abc";
        private const string State = "test";

        [Test]
        public void Create_Arbitrary()
        {
            foreach (var arguments in new[] { new Value[] { 1 }, new Value[] { 1, 2 }, new Value[] { 1, 2, 3 } })
            {
                var function = Function.Create(FunctionTester.CreateImpureCallback(arguments));

                FunctionTester.AssertImpure(function, arguments, FunctionTester.Output);
            }
        }

        [Test]
        public void Create_Exact()
        {
            var arguments = new Value[] { 1, 2, 3 };
            var function = Function.Create(FunctionTester.CreateImpureCallback(arguments), 3);

            FunctionTester.AssertImpure(function, arguments, FunctionTester.Output);
            FunctionTester.AssertImpureFailure(function, new Value[] { 1, 2 });
            FunctionTester.AssertImpureFailure(function, new Value[] { 1, 2, 3, 4 });
        }

        [Test]
        public void Create_Range()
        {
            var arguments2 = new Value[] { 1, 2 };
            var function2 = Function.Create(FunctionTester.CreateImpureCallback(arguments2), 2, 3);

            FunctionTester.AssertImpure(function2, arguments2, FunctionTester.Output);
            FunctionTester.AssertImpureFailure(function2, new Value[] { 1 });
            FunctionTester.AssertImpureFailure(function2, new Value[] { 1, 2, 3, 4 });

            var arguments3 = new Value[] { 1, 2, 3 };
            var function3 = Function.Create(FunctionTester.CreateImpureCallback(arguments3), 2, 3);

            FunctionTester.AssertImpure(function3, arguments3, FunctionTester.Output);
            FunctionTester.AssertImpureFailure(function3, new Value[] { 1 });
            FunctionTester.AssertImpureFailure(function3, new Value[] { 1, 2, 3, 4 });
        }

        [Test]
        public void Create1()
        {
            var argument = (Value)17;
            var function = Function.Create1((state, a1, output) =>
            {
                Assert.That(a1, Is.EqualTo(argument));

                return FunctionTester.Return;
            });

            FunctionTester.AssertImpure(function, new[] { argument }, string.Empty);
            FunctionTester.AssertImpureFailure(function, new Value[] { 1, 2 });
        }

        [Test]
        public void Create2()
        {
            var argument1 = (Value)17;
            var argument2 = (Value)23;
            var function = Function.Create2((state, a1, a2, output) =>
            {
                Assert.That(a1, Is.EqualTo(argument1));
                Assert.That(a2, Is.EqualTo(argument2));

                return FunctionTester.Return;
            });

            FunctionTester.AssertImpure(function, new[] { argument1, argument2 }, string.Empty);
            FunctionTester.AssertImpureFailure(function, new Value[] { 1 });
        }

        [Test]
        public void CreatePure_Arbitrary()
        {
            foreach (var arguments in new[] { new Value[] { 1 }, new Value[] { 1, 2 }, new Value[] { 1, 2, 3 } })
            {
                var function = Function.CreatePure(FunctionTester.CreatePureCallback(arguments));

                FunctionTester.AssertPure(function, arguments);
            }
        }

        [Test]
        public void CreatePure_Exact()
        {
            var arguments = new Value[] { 1, 2, 3 };
            var function = Function.CreatePure(FunctionTester.CreatePureCallback(arguments), 3);

            FunctionTester.AssertPure(function, arguments);
            FunctionTester.AssertPureFailure(function, new Value[] { 1, 2 });
            FunctionTester.AssertPureFailure(function, new Value[] { 1, 2, 3, 4 });
        }

        [Test]
        public void CreatePure_Range()
        {
            var arguments2 = new Value[] { 1, 2 };
            var function2 = Function.CreatePure(FunctionTester.CreatePureCallback(arguments2), 2, 3);

            FunctionTester.AssertPure(function2, arguments2);
            FunctionTester.AssertPureFailure(function2, new Value[] { 1 });
            FunctionTester.AssertPureFailure(function2, new Value[] { 1, 2, 3, 4 });

            var arguments3 = new Value[] { 1, 2, 3 };
            var function3 = Function.CreatePure(FunctionTester.CreatePureCallback(arguments3), 2, 3);

            FunctionTester.AssertPure(function3, arguments3);
            FunctionTester.AssertPureFailure(function3, new Value[] { 1 });
            FunctionTester.AssertPureFailure(function3, new Value[] { 1, 2, 3, 4 });
        }

        [Test]
        public void CreatePure1()
        {
            var argument = (Value)17;
            var function = Function.CreatePure1((state, a1) =>
            {
                Assert.That(a1, Is.EqualTo(argument));

                return FunctionTester.Return;
            });

            FunctionTester.AssertPure(function, new[] { argument });
            FunctionTester.AssertPureFailure(function, new Value[] { 1, 2 });
        }

        [Test]
        public void CreatePure2()
        {
            var argument1 = (Value)17;
            var argument2 = (Value)23;
            var function = Function.CreatePure2((state, a1, a2) =>
            {
                Assert.That(a1, Is.EqualTo(argument1));
                Assert.That(a2, Is.EqualTo(argument2));

                return FunctionTester.Return;
            });

            FunctionTester.AssertPure(function, new[] { argument1, argument2 });
            FunctionTester.AssertPureFailure(function, new Value[] { 1 });
        }

        private static void AssertImpure(IFunction function, IReadOnlyList<Value> arguments, string output)
        {
            using (var writer = new StringWriter())
            {
                var result = function.Invoke(FunctionTester.State, arguments, writer);

                Assert.That(function.IsPure, Is.False);
                Assert.That(result, Is.EqualTo(FunctionTester.Return));
                Assert.That(writer.ToString(), Is.EqualTo(output));
            }
        }

        private static void AssertImpureFailure(IFunction function, IReadOnlyList<Value> arguments)
        {
            using (var writer = new StringWriter())
            {
                var result = function.Invoke(FunctionTester.State, arguments, writer);

                Assert.That(function.IsPure, Is.False);
                Assert.That(result, Is.EqualTo(VoidValue.Instance));
                Assert.That(writer.ToString(), Is.Empty);
            }
        }

        private static void AssertPure(IFunction function, IReadOnlyList<Value> arguments)
        {
            using (var writer = new StringWriter())
            {
                var result = function.Invoke(FunctionTester.State, arguments, writer);

                Assert.That(function.IsPure, Is.True);
                Assert.That(result, Is.EqualTo(FunctionTester.Return));
                Assert.That(writer.ToString(), Is.Empty);
            }
        }

        private static void AssertPureFailure(IFunction function, IReadOnlyList<Value> arguments)
        {
            using (var writer = new StringWriter())
            {
                var result = function.Invoke(FunctionTester.State, arguments, writer);

                Assert.That(function.IsPure, Is.True);
                Assert.That(result, Is.EqualTo(VoidValue.Instance));
                Assert.That(writer.ToString(), Is.Empty);
            }
        }

        private static Func<object, IReadOnlyList<Value>, TextWriter, Value> CreateImpureCallback(Value[] expected)
        {
            return (state, arguments, output) =>
            {
                Assert.That(arguments, Is.EqualTo(expected));
                Assert.That(state, Is.EqualTo(FunctionTester.State));

                output.Write(new[] { 'a', 'b', 'c' });

                return FunctionTester.Return;
            };
        }

        private static Func<object, IReadOnlyList<Value>, Value> CreatePureCallback(Value[] expected)
        {
            return (state, arguments) =>
            {
                Assert.That(arguments, Is.EqualTo(expected));
                Assert.That(state, Is.EqualTo(FunctionTester.State));

                return FunctionTester.Return;
            };
        }
    }
}