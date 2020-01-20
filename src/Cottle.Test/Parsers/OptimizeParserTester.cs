using System.IO;
using System.Linq;
using Cottle.Parsers;
using Cottle.Values;
using Moq;
using NUnit.Framework;

namespace Cottle.Test.Parsers
{
    public class OptimizeParserTester
    {
        [Test]
        public void Parse_OptimizeAccess_FindSymbolFromConstantIndices()
        {
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(new Expression
            {
                Source = OptimizeParserTester.CreateExpressionMap(new[]
                {
                    new ExpressionElement(OptimizeParserTester.CreateExpressionConstant(0),
                        OptimizeParserTester.CreateExpressionSymbol("AAA")),
                    new ExpressionElement(OptimizeParserTester.CreateExpressionConstant(1),
                        OptimizeParserTester.CreateExpressionSymbol("BBB")),
                    new ExpressionElement(OptimizeParserTester.CreateExpressionConstant(2),
                        OptimizeParserTester.CreateExpressionSymbol("CCC"))
                }),
                Subscript = OptimizeParserTester.CreateExpressionConstant(1),
                Type = ExpressionType.Access
            }));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Symbol));
            Assert.That(command.Operand.Value, Is.EqualTo((Value)"BBB"));
        }

        [Test]
        public void Parse_OptimizeAccess_FindVoidFromConstantIndices()
        {
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(new Expression
            {
                Source = OptimizeParserTester.CreateExpressionMap(new[]
                {
                    new ExpressionElement(OptimizeParserTester.CreateExpressionConstant(0),
                        OptimizeParserTester.CreateExpressionSymbol("AAA")),
                    new ExpressionElement(OptimizeParserTester.CreateExpressionConstant(1),
                        OptimizeParserTester.CreateExpressionSymbol("BBB")),
                    new ExpressionElement(OptimizeParserTester.CreateExpressionConstant(2),
                        OptimizeParserTester.CreateExpressionSymbol("CCC"))
                }),
                Subscript = OptimizeParserTester.CreateExpressionConstant(3),
                Type = ExpressionType.Access
            }));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(command.Operand.Value, Is.EqualTo(VoidValue.Instance));
        }

        [Test]
        public void Parse_OptimizeAccess_SkipFromVariableIndices()
        {
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(new Expression
            {
                Source = OptimizeParserTester.CreateExpressionMap(new[]
                {
                    new ExpressionElement(OptimizeParserTester.CreateExpressionConstant(0),
                        OptimizeParserTester.CreateExpressionSymbol("AAA")),
                    new ExpressionElement(OptimizeParserTester.CreateExpressionConstant(1),
                        OptimizeParserTester.CreateExpressionSymbol("BBB")),
                    new ExpressionElement(OptimizeParserTester.CreateExpressionSymbol("x"),
                        OptimizeParserTester.CreateExpressionSymbol("CCC"))
                }),
                Subscript = OptimizeParserTester.CreateExpressionConstant(3),
                Type = ExpressionType.Access
            }));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Access));
        }

        [Test]
        public void Parse_OptimizeInvoke_CallPureFunction()
        {
            var function = Function.CreatePure2((state, a, b) => 3);
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(new Expression
            {
                Arguments = new[]
                {
                    OptimizeParserTester.CreateExpressionConstant(1),
                    OptimizeParserTester.CreateExpressionConstant(2)
                },
                Source = OptimizeParserTester.CreateExpressionConstant(new FunctionValue(function)),
                Type = ExpressionType.Invoke
            }));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(command.Operand.Value, Is.EqualTo((Value)3));
        }

        [Test]
        public void Parse_OptimizeInvoke_SkipImpureFunction()
        {
            var function = Function.Create1((state, value, output) => VoidValue.Instance);
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(new Expression
            {
                Arguments = new[]
                {
                    OptimizeParserTester.CreateExpressionConstant(1),
                    OptimizeParserTester.CreateExpressionConstant(2)
                },
                Source = OptimizeParserTester.CreateExpressionConstant(new FunctionValue(function)),
                Type = ExpressionType.Invoke
            }));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Invoke));
        }

        [Test]
        public void Parse_OptimizeInvoke_SkipSymbolArgument()
        {
            var function = Function.CreatePure1((state, value) => VoidValue.Instance);
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(new Expression
            {
                Arguments = new[]
                {
                    OptimizeParserTester.CreateExpressionConstant(1),
                    OptimizeParserTester.CreateExpressionSymbol("x")
                },
                Source = OptimizeParserTester.CreateExpressionConstant(new FunctionValue(function)),
                Type = ExpressionType.Invoke
            }));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Invoke));
        }

        private static Expression CreateExpressionConstant(Value value)
        {
            return new Expression { Type = ExpressionType.Constant, Value = value };
        }

        private static Expression CreateExpressionMap(ExpressionElement[] elements)
        {
            return new Expression
            {
                Elements = elements,
                Type = ExpressionType.Map
            };
        }

        private static Expression CreateExpressionSymbol(string name)
        {
            return new Expression { Type = ExpressionType.Symbol, Value = name };
        }

        private static Command CreateCommandEcho(Expression operand)
        {
            return new Command { Operand = operand, Type = CommandType.Echo };
        }

        private static Command Optimize(Command command)
        {
            var parserMock = new Mock<IParser>();
            var reports = Enumerable.Empty<DocumentReport>();

            parserMock.Setup(p => p.Parse(It.IsAny<TextReader>(), out command, out reports)).Returns(true);

            var parser = new OptimizeParser(parserMock.Object);

            Assert.That(parser.Parse(TextReader.Null, out var output, out _), Is.True);

            return output;
        }
    }
}