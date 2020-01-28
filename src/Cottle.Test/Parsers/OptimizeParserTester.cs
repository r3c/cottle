using System;
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
        private static readonly Expression ImpureFunction =
            Expression.CreateConstant(
                new FunctionValue(Function.Create((state, arguments, output) => VoidValue.Instance)));

        [Test]
        public void Parse_OptimizeAccess_FindWhenPresentInConstantIndices()
        {
            var expression = OptimizeParserTester.Optimize(Expression.CreateAccess(Expression.CreateMap(
                    new[]
                    {
                        new ExpressionElement(Expression.CreateConstant(0),
                            Expression.CreateSymbol("AAA")),
                        new ExpressionElement(Expression.CreateConstant(1),
                            Expression.CreateSymbol("BBB")),
                        new ExpressionElement(Expression.CreateConstant(2),
                            Expression.CreateSymbol("CCC"))
                    }),
                Expression.CreateConstant(1)
            ));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Symbol));
            Assert.That(expression.Value, Is.EqualTo((Value)"BBB"));
        }

        [Test]
        public void Parse_OptimizeAccess_FindWhenMissingFromConstantIndices()
        {
            var expression = OptimizeParserTester.Optimize(Expression.CreateAccess(Expression.CreateMap(
                    new[]
                    {
                        new ExpressionElement(Expression.CreateConstant(0),
                            Expression.CreateSymbol("AAA")),
                        new ExpressionElement(Expression.CreateConstant(1),
                            Expression.CreateSymbol("BBB")),
                        new ExpressionElement(Expression.CreateConstant(2),
                            Expression.CreateSymbol("CCC"))
                    }),
                Expression.CreateConstant(3)
            ));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(expression.Value, Is.EqualTo(VoidValue.Instance));
        }

        [Test]
        public void Parse_OptimizeAccess_SkipWhenNonPureIndices()
        {
            var expression = OptimizeParserTester.Optimize(Expression.CreateAccess(Expression.CreateMap(
                    new[]
                    {
                        new ExpressionElement(Expression.CreateConstant(0),
                            Expression.CreateInvoke(OptimizeParserTester.ImpureFunction, Array.Empty<Expression>())),
                        new ExpressionElement(Expression.CreateConstant(1),
                            Expression.CreateSymbol("BBB")),
                        new ExpressionElement(Expression.CreateSymbol("x"),
                            Expression.CreateSymbol("CCC"))
                    }),
                Expression.CreateConstant(3)
            ));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Access));
        }

        [Test]
        public void Parse_OptimizeInvoke_CallPureFunction()
        {
            var function = Function.CreatePure2((state, a, b) => 3);
            var expression = OptimizeParserTester.Optimize(Expression.CreateInvoke(
                Expression.CreateConstant(new FunctionValue(function)),
                new[] { Expression.CreateConstant(1), Expression.CreateConstant(2) }));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(expression.Value, Is.EqualTo((Value)3));
        }

        [Test]
        public void Parse_OptimizeInvoke_ResolveNotAFunction()
        {
            var expression =
                OptimizeParserTester.Optimize(Expression.CreateInvoke(Expression.CreateConstant(1),
                    Array.Empty<Expression>()));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(expression.Value, Is.EqualTo(VoidValue.Instance));
        }

        [Test]
        public void Parse_OptimizeInvoke_SkipImpureFunction()
        {
            var expression = OptimizeParserTester.Optimize(Expression.CreateInvoke(OptimizeParserTester.ImpureFunction,
                new[]
                {
                    Expression.CreateConstant(1),
                    Expression.CreateConstant(2)
                }));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Invoke));
        }

        [Test]
        public void Parse_OptimizeInvoke_SkipSymbolArgument()
        {
            var expression = OptimizeParserTester.Optimize(Expression.CreateInvoke(OptimizeParserTester.ImpureFunction,
                new[]
                {
                    Expression.CreateConstant(1),
                    Expression.CreateSymbol("x"),
                }));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Invoke));
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

        private static Expression Optimize(Expression expression)
        {
            var command = OptimizeParserTester.Optimize(Command.CreateEcho(expression));

            Assert.That(command.Type, Is.EqualTo(CommandType.Echo));

            return command.Operand;
        }
    }
}