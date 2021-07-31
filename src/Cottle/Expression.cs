using System;
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

        public static Expression CreateSymbol(string value)
        {
            return new Expression(ExpressionType.Symbol, default, default, default, default, value);
        }

        public static readonly Expression Void = Expression.CreateConstant(Value.Undefined);

        public IReadOnlyList<Expression> Arguments => _arguments ?? Array.Empty<Expression>();

        public IReadOnlyList<ExpressionElement> Elements => _elements ?? Array.Empty<ExpressionElement>();

        public Expression Source => _source ?? Expression.Void;

        public Expression Subscript => _subscript ?? Expression.Void;

        public readonly ExpressionType Type;

        public Value Value => _value ?? Value.Undefined;

        private readonly IReadOnlyList<Expression>? _arguments;

        private readonly IReadOnlyList<ExpressionElement>? _elements;

        private readonly Expression? _source;

        private readonly Expression? _subscript;

        private readonly Value? _value;

        private Expression(ExpressionType type, IReadOnlyList<Expression>? arguments,
            IReadOnlyList<ExpressionElement>? elements, Expression? source, Expression? subscript, Value? value)
        {
            Type = type;

            _arguments = arguments;
            _elements = elements;
            _source = source;
            _subscript = subscript;
            _value = value;
        }
    }
}