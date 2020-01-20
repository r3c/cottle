using System.Collections.Generic;

namespace Cottle
{
    internal class Expression
    {
        public static Expression CreateAccess(Expression source, Expression subscript)
        {
            return new Expression(ExpressionType.Access, default, default, source, subscript, default);
        }

        public static Expression CreateConstant(Value value)
        {
            return new Expression(ExpressionType.Constant, default, default, default, default, value);
        }

        public static Expression CreateInvoke(Expression source, IReadOnlyList<Expression> arguments)
        {
            return new Expression(ExpressionType.Invoke, arguments, default, source, default, default);
        }

        public static Expression CreateMap(IReadOnlyList<ExpressionElement> elements)
        {
            return new Expression(ExpressionType.Map, default, elements, default, default, default);
        }

        public static Expression CreateSymbol(Value value)
        {
            return new Expression(ExpressionType.Symbol, default, default, default, default, value);
        }

        public static readonly Expression Void = new Expression(ExpressionType.Void, default, default, default, default,
            default);        

        public readonly IReadOnlyList<Expression> Arguments;

        public readonly IReadOnlyList<ExpressionElement> Elements;

        public readonly Expression Source;

        public readonly Expression Subscript;

        public readonly ExpressionType Type;

        public readonly Value Value;

        private Expression(ExpressionType type, IReadOnlyList<Expression> arguments,
            IReadOnlyList<ExpressionElement> elements, Expression source, Expression subscript, Value value)
        {
            Arguments = arguments;
            Elements = elements;
            Source = source;
            Subscript = subscript;
            Type = type;
            Value = value;
        }
    }
}