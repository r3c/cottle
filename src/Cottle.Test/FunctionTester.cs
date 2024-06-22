using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Cottle.Test
{
    public class FunctionTester
    {
        private static readonly Value Return = 42;
        private const string Output = "abc";
        private const object? Runtime = null;

        [Test]
        public void CreateNative0()
        {
            var function = Function.CreateNative0((_, _) => FunctionTester.Return);

            FunctionTester.AssertNative(function, Array.Empty<Value>(), string.Empty);
            FunctionTester.AssertNativeFailure(function, new Value[] { 1 });
        }

        [Test]
        public void CreateNative1()
        {
            var argument = (Value)17;
            var function = Function.CreateNative1((_, a0, _) =>
            {
                Assert.That(a0, Is.EqualTo(argument));

                return FunctionTester.Return;
            });

            FunctionTester.AssertNative(function, new[] { argument }, string.Empty);
            FunctionTester.AssertNativeFailure(function, new Value[] { 1, 2 });
        }

        [Test]
        public void CreateNative2()
        {
            var argument0 = (Value)17;
            var argument1 = (Value)23;
            var function = Function.CreateNative2((_, a0, a1, _) =>
            {
                Assert.That(a0, Is.EqualTo(argument0));
                Assert.That(a1, Is.EqualTo(argument1));

                return FunctionTester.Return;
            });

            FunctionTester.AssertNative(function, new[] { argument0, argument1 }, string.Empty);
            FunctionTester.AssertNativeFailure(function, new Value[] { 1 });
        }

        [Test]
        public void Create3()
        {
            var argument0 = (Value)17;
            var argument1 = (Value)23;
            var argument2 = (Value)42;
            var function = Function.CreateNative3((_, a0, a1, a2, _) =>
            {
                Assert.That(a0, Is.EqualTo(argument0));
                Assert.That(a1, Is.EqualTo(argument1));
                Assert.That(a2, Is.EqualTo(argument2));

                return FunctionTester.Return;
            });

            FunctionTester.AssertNative(function, new[] { argument0, argument1, argument2 }, string.Empty);
            FunctionTester.AssertNativeFailure(function, new Value[] { 1, 2 });
        }

        [Test]
        public void CreateNativeExact()
        {
            var arguments = new Value[] { 1, 2, 3 };
            var function = Function.CreateNativeExact(FunctionTester.CreateNativeCallback(arguments), 3);

            FunctionTester.AssertNative(function, arguments, FunctionTester.Output);
            FunctionTester.AssertNativeFailure(function, new Value[] { 1, 2 });
            FunctionTester.AssertNativeFailure(function, new Value[] { 1, 2, 3, 4 });
        }

        [Test]
        public void CreateNativeMinMax()
        {
            var arguments2 = new Value[] { 1, 2 };
            var function2 = Function.CreateNativeMinMax(FunctionTester.CreateNativeCallback(arguments2), 2, 3);

            FunctionTester.AssertNative(function2, arguments2, FunctionTester.Output);
            FunctionTester.AssertNativeFailure(function2, new Value[] { 1 });
            FunctionTester.AssertNativeFailure(function2, new Value[] { 1, 2, 3, 4 });

            var arguments3 = new Value[] { 1, 2, 3 };
            var function3 = Function.CreateNativeMinMax(FunctionTester.CreateNativeCallback(arguments3), 2, 3);

            FunctionTester.AssertNative(function3, arguments3, FunctionTester.Output);
            FunctionTester.AssertNativeFailure(function3, new Value[] { 1 });
            FunctionTester.AssertNativeFailure(function3, new Value[] { 1, 2, 3, 4 });
        }

        [Test]
        public void CreateNativeVariadic()
        {
            foreach (var arguments in new[] { new Value[] { 1 }, new Value[] { 1, 2 }, new Value[] { 1, 2, 3 } })
            {
                var function = Function.CreateNativeVariadic(FunctionTester.CreateNativeCallback(arguments));

                FunctionTester.AssertNative(function, arguments, FunctionTester.Output);
            }
        }

        [Test]
        public void CreatePure0()
        {
            var function = Function.CreatePure0((_) => FunctionTester.Return);

            FunctionTester.AssertPure(function, Array.Empty<Value>());
            FunctionTester.AssertPureFailure(function, new Value[] { 1 });
        }

        [Test]
        public void CreatePure1()
        {
            var argument = (Value)17;
            var function = Function.CreatePure1((_, a0) =>
            {
                Assert.That(a0, Is.EqualTo(argument));

                return FunctionTester.Return;
            });

            FunctionTester.AssertPure(function, new[] { argument });
            FunctionTester.AssertPureFailure(function, new Value[] { 1, 2 });
        }

        [Test]
        public void CreatePure2()
        {
            var argument0 = (Value)17;
            var argument1 = (Value)23;
            var function = Function.CreatePure2((_, a0, a1) =>
            {
                Assert.That(a0, Is.EqualTo(argument0));
                Assert.That(a1, Is.EqualTo(argument1));

                return FunctionTester.Return;
            });

            FunctionTester.AssertPure(function, new[] { argument0, argument1 });
            FunctionTester.AssertPureFailure(function, new Value[] { 1 });
        }

        [Test]
        public void CreatePure3()
        {
            var argument0 = (Value)17;
            var argument1 = (Value)23;
            var argument2 = (Value)42;
            var function = Function.CreatePure3((_, a0, a1, a2) =>
            {
                Assert.That(a0, Is.EqualTo(argument0));
                Assert.That(a1, Is.EqualTo(argument1));
                Assert.That(a2, Is.EqualTo(argument2));

                return FunctionTester.Return;
            });

            FunctionTester.AssertPure(function, new[] { argument0, argument1, argument2 });
            FunctionTester.AssertPureFailure(function, new Value[] { 1, 2 });
        }

        [Test]
        public void CreatePureExact()
        {
            var arguments = new Value[] { 1, 2, 3 };
            var function = Function.CreatePureExact(FunctionTester.CreatePureCallback(arguments), 3);

            FunctionTester.AssertPure(function, arguments);
            FunctionTester.AssertPureFailure(function, new Value[] { 1, 2 });
            FunctionTester.AssertPureFailure(function, new Value[] { 1, 2, 3, 4 });
        }

        [Test]
        public void CreatePureMinMax()
        {
            var arguments2 = new Value[] { 1, 2 };
            var function2 = Function.CreatePureMinMax(FunctionTester.CreatePureCallback(arguments2), 2, 3);

            FunctionTester.AssertPure(function2, arguments2);
            FunctionTester.AssertPureFailure(function2, new Value[] { 1 });
            FunctionTester.AssertPureFailure(function2, new Value[] { 1, 2, 3, 4 });

            var arguments3 = new Value[] { 1, 2, 3 };
            var function3 = Function.CreatePureMinMax(FunctionTester.CreatePureCallback(arguments3), 2, 3);

            FunctionTester.AssertPure(function3, arguments3);
            FunctionTester.AssertPureFailure(function3, new Value[] { 1 });
            FunctionTester.AssertPureFailure(function3, new Value[] { 1, 2, 3, 4 });
        }

        [Test]
        public void CreatePureVariadic()
        {
            foreach (var arguments in new[] { new Value[] { 1 }, new Value[] { 1, 2 }, new Value[] { 1, 2, 3 } })
            {
                var function = Function.CreatePureVariadic(FunctionTester.CreatePureCallback(arguments));

                FunctionTester.AssertPure(function, arguments);
            }
        }

        private static void AssertNative(IFunction function, IReadOnlyList<Value> arguments, string output)
        {
            using var writer = new StringWriter();

            var result = function.Invoke(FunctionTester.Runtime, arguments, writer);

            Assert.That(function.IsPure, Is.False);
            Assert.That(result, Is.EqualTo(FunctionTester.Return));
            Assert.That(writer.ToString(), Is.EqualTo(output));
        }

        private static void AssertNativeFailure(IFunction function, IReadOnlyList<Value> arguments)
        {
            using var writer = new StringWriter();

            var result = function.Invoke(FunctionTester.Runtime, arguments, writer);

            Assert.That(function.IsPure, Is.False);
            Assert.That(result, Is.EqualTo(Value.Undefined));
            Assert.That(writer.ToString(), Is.Empty);
        }

        private static void AssertPure(IFunction function, IReadOnlyList<Value> arguments)
        {
            using var writer = new StringWriter();

            var result = function.Invoke(FunctionTester.Runtime, arguments, writer);

            Assert.That(function.IsPure, Is.True);
            Assert.That(result, Is.EqualTo(FunctionTester.Return));
            Assert.That(writer.ToString(), Is.Empty);
        }

        private static void AssertPureFailure(IFunction function, IReadOnlyList<Value> arguments)
        {
            using var writer = new StringWriter();

            var result = function.Invoke(FunctionTester.Runtime, arguments, writer);

            Assert.That(function.IsPure, Is.True);
            Assert.That(result, Is.EqualTo(Value.Undefined));
            Assert.That(writer.ToString(), Is.Empty);
        }

        private static Func<object, IReadOnlyList<Value>, TextWriter, Value> CreateNativeCallback(Value[] expected)
        {
            return (state, arguments, output) =>
            {
                Assert.That(arguments, Is.EqualTo(expected));
                Assert.That(state, Is.EqualTo(FunctionTester.Runtime));

                output.Write(new[] { 'a', 'b', 'c' });

                return FunctionTester.Return;
            };
        }

        private static Func<object?, IReadOnlyList<Value>, Value> CreatePureCallback(Value[] expected)
        {
            return (state, arguments) =>
            {
                Assert.That(arguments, Is.EqualTo(expected));
                Assert.That(state, Is.EqualTo(FunctionTester.Runtime));

                return FunctionTester.Return;
            };
        }
    }
}