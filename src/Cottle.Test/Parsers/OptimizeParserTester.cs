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
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(Expression.CreateAccess(
                Expression.CreateMap(new[]
                {
                    new ExpressionElement(Expression.CreateConstant(0),
                        Expression.CreateSymbol("AAA")),
                    new ExpressionElement(Expression.CreateConstant(1),
                        Expression.CreateSymbol("BBB")),
                    new ExpressionElement(Expression.CreateConstant(2),
                        Expression.CreateSymbol("CCC"))
                }),
                Expression.CreateConstant(1)
            )));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Symbol));
            Assert.That(command.Operand.Value, Is.EqualTo((Value)"BBB"));
        }

        [Test]
        public void Parse_OptimizeAccess_FindVoidFromConstantIndices()
        {
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(Expression.CreateAccess(
                Expression.CreateMap(new[]
                {
                    new ExpressionElement(Expression.CreateConstant(0),
                        Expression.CreateSymbol("AAA")),
                    new ExpressionElement(Expression.CreateConstant(1),
                        Expression.CreateSymbol("BBB")),
                    new ExpressionElement(Expression.CreateConstant(2),
                        Expression.CreateSymbol("CCC"))
                }),
                Expression.CreateConstant(3)
            )));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(command.Operand.Value, Is.EqualTo(VoidValue.Instance));
        }

        [Test]
        public void Parse_OptimizeAccess_SkipFromVariableIndices()
        {
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(Expression.CreateAccess(
                Expression.CreateMap(new[]
                {
                    new ExpressionElement(Expression.CreateConstant(0),
                        Expression.CreateSymbol("AAA")),
                    new ExpressionElement(Expression.CreateConstant(1),
                        Expression.CreateSymbol("BBB")),
                    new ExpressionElement(Expression.CreateSymbol("x"),
                        Expression.CreateSymbol("CCC"))
                }), 
                Expression.CreateConstant(3)
            )));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Access));
        }

        [Test]
        public void Parse_OptimizeInvoke_CallPureFunction()
        {
            var function = Function.CreatePure2((state, a, b) => 3);
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(Expression.CreateInvoke(
                Expression.CreateConstant(new FunctionValue(function)), new[]
                {
                    Expression.CreateConstant(1),
                    Expression.CreateConstant(2)
                })));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(command.Operand.Value, Is.EqualTo((Value)3));
        }

        [Test]
        public void Parse_OptimizeInvoke_SkipImpureFunction()
        {
            var function = Function.Create1((state, value, output) => VoidValue.Instance);
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(Expression.CreateInvoke(
                Expression.CreateConstant(new FunctionValue(function)), new[]
                {
                    Expression.CreateConstant(1),
                    Expression.CreateConstant(2)
                })));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Invoke));
        }

        [Test]
        public void Parse_OptimizeInvoke_SkipSymbolArgument()
        {
            var function = Function.CreatePure1((state, value) => VoidValue.Instance);
            var command = OptimizeParserTester.Optimize(OptimizeParserTester.CreateCommandEcho(Expression.CreateInvoke(
                Expression.CreateConstant(new FunctionValue(function)), new[]
                {
                    Expression.CreateConstant(1),
                    Expression.CreateSymbol("x"),
                })));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));
            Assert.That(command.Operand.Type, Is.EqualTo(ExpressionType.Invoke));
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