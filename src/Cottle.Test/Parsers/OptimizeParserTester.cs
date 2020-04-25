using System;
using System.IO;
using System.Linq;
using Cottle.Parsers;
using Moq;
using NUnit.Framework;

namespace Cottle.Test.Parsers
{
    public class OptimizeParserTester
    {
        private static readonly Expression ImpureFunction =
            Expression.CreateConstant(
                Value.FromFunction(Function.Create((state, arguments, output) => Value.Undefined)));

        private static readonly Expression PureFunction =
            Expression.CreateConstant(Value.FromFunction(Function.CreatePure((s, a) => 0)));

        [Test]
        public void Parse_ExpressionAccess_FindWhenPresentInConstantIndices()
        {
            // Expression: [0: AAA, 1: BBB, 2: pure()][1]
            // Result: BBB
            var expression = OptimizeParserTester.Optimize(Expression.CreateAccess(Expression.CreateMap(
                    new[]
                    {
                        new ExpressionElement(Expression.CreateConstant(0),
                            Expression.CreateSymbol("AAA")),
                        new ExpressionElement(Expression.CreateConstant(1),
                            Expression.CreateSymbol("BBB")),
                        new ExpressionElement(Expression.CreateConstant(2),
                            Expression.CreateInvoke(OptimizeParserTester.PureFunction, Array.Empty<Expression>()))
                    }),
                Expression.CreateConstant(1)
            ));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Symbol));
            Assert.That(expression.Value, Is.EqualTo((Value)"BBB"));
        }

        [Test]
        public void Parse_ExpressionAccess_FindWhenMissingFromConstantIndices()
        {
            // Expression: [0: AAA, 1: BBB, 2: CCC][3]
            // Result: <void>
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
            Assert.That(expression.Value, Is.EqualTo(Value.Undefined));
        }

        [Test]
        public void Parse_ExpressionAccess_FindNestedConstantExpressions()
        {
            // Expression: [17: [3: 42][3]][[2: 17][2]]
            // Result: 42
            const int value = 42;

            var index1 = Expression.CreateConstant(17);
            var index2 = Expression.CreateConstant(3);
            var index3 = Expression.CreateConstant(2);

            var expression = OptimizeParserTester.Optimize(Expression.CreateAccess(
                Expression.CreateMap(new[]
                {
                    new ExpressionElement(index1,
                        Expression.CreateAccess(
                            Expression.CreateMap(new[]
                                { new ExpressionElement(index2, Expression.CreateConstant(value)) }), index2))
                }),
                Expression.CreateAccess(Expression.CreateMap(new[] { new ExpressionElement(index3, index1) }),
                    index3)));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(expression.Value, Is.EqualTo((Value)value));
        }

        [Test]
        public void Parse_ExpressionAccess_SkipWhenNonPureIndices()
        {
            // Expression: [0: AAA, 1: BBB, x: CCC][3]
            var expression = OptimizeParserTester.Optimize(Expression.CreateAccess(Expression.CreateMap(
                    new[]
                    {
                        new ExpressionElement(Expression.CreateConstant(0),
                            Expression.CreateSymbol("AAA")),
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
        public void Parse_ExpressionMap_FoldConstant()
        {
            // Expression: [0: "X", 1: "Y", x: "Z"]
            // Result: <map>
            var expression = OptimizeParserTester.Optimize(Expression.CreateMap(new[]
            {
                new ExpressionElement(Expression.CreateConstant(0), Expression.CreateConstant("X")),
                new ExpressionElement(Expression.CreateConstant(1), Expression.CreateConstant("Y")),
                new ExpressionElement(Expression.CreateConstant(2), Expression.CreateConstant("Z"))
            }));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(expression.Value, Is.EqualTo((Value)new Value[] { "X", "Y", "Z" }));
        }

        [Test]
        public void Parse_ExpressionMap_SkipDynamic()
        {
            // Expression: [0: "X", 1: "Y", x: "Z"]
            var expression = OptimizeParserTester.Optimize(Expression.CreateMap(new[]
            {
                new ExpressionElement(Expression.CreateConstant(0), Expression.CreateConstant("X")),
                new ExpressionElement(Expression.CreateConstant(1), Expression.CreateSymbol("Y")),
                new ExpressionElement(Expression.CreateConstant(2), Expression.CreateConstant("Z"))
            }));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Map));
        }

        [Test]
        public void Parse_ExpressionInvoke_CallPureFunction()
        {
            // Expression: pure(1, 2)
            // Result: 3
            var function = Function.CreatePure2((state, a, b) => 3);
            var expression = OptimizeParserTester.Optimize(Expression.CreateInvoke(
                Expression.CreateConstant(Value.FromFunction(function)),
                new[] { Expression.CreateConstant(1), Expression.CreateConstant(2) }));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(expression.Value, Is.EqualTo((Value)3));
        }

        [Test]
        public void Parse_ExpressionInvoke_ResolveNotAFunction()
        {
            // Expression: 1()
            // Result: <void>
            var expression =
                OptimizeParserTester.Optimize(Expression.CreateInvoke(Expression.CreateConstant(1),
                    Array.Empty<Expression>()));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(expression.Value, Is.EqualTo(Value.Undefined));
        }

        [Test]
        public void Parse_ExpressionInvoke_SkipImpureFunction()
        {
            // Expression: impure(1, 2)
            var expression = OptimizeParserTester.Optimize(Expression.CreateInvoke(OptimizeParserTester.ImpureFunction,
                new[]
                {
                    Expression.CreateConstant(1),
                    Expression.CreateConstant(2)
                }));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Invoke));
        }

        [Test]
        public void Parse_ExpressionInvoke_SkipSymbolArgument()
        {
            // Expression: pure(1, x)
            var expression = OptimizeParserTester.Optimize(Expression.CreateInvoke(OptimizeParserTester.PureFunction,
                new[]
                {
                    Expression.CreateConstant(1),
                    Expression.CreateSymbol("x"),
                }));

            Assert.That(expression.Type, Is.EqualTo(ExpressionType.Invoke));
        }

        [Test]
        public void Parse_StatementReturn()
        {
            // Statement: X{return 1}Y
            // Result: X{return 1}
            var statement = OptimizeParserTester.Optimize(Statement.CreateComposite(Statement.CreateLiteral("X"),
                Statement.CreateComposite(Statement.CreateReturn(Expression.CreateConstant(1)),
                    Statement.CreateLiteral("Y"))));

            Assert.That(statement.Type, Is.EqualTo(StatementType.Composite));
            Assert.That(statement.Body.Type, Is.EqualTo(StatementType.Literal));
            Assert.That(statement.Body.Value, Is.EqualTo("X"));
            Assert.That(statement.Next.Type, Is.EqualTo(StatementType.Return));
            Assert.That(statement.Next.Operand.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(statement.Next.Operand.Value, Is.EqualTo((Value)1));
        }

        private static Statement Optimize(Statement statement)
        {
            var parserMock = new Mock<IParser>();
            var reports = Enumerable.Empty<DocumentReport>();

            parserMock.Setup(p => p.Parse(It.IsAny<TextReader>(), out statement, out reports)).Returns(true);

            var parser = new OptimizeParser(parserMock.Object);

            Assert.That(parser.Parse(TextReader.Null, out var output, out _), Is.True);

            return output;
        }

        private static Expression Optimize(Expression expression)
        {
            var statement = OptimizeParserTester.Optimize(Statement.CreateEcho(expression));

            Assert.That(statement.Type, Is.EqualTo(StatementType.Echo));

            return statement.Operand;
        }
    }
}