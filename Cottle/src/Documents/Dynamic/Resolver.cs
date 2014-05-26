using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cottle.Documents.Dynamic
{
	static class Resolver
	{
		public static ConstructorInfo Constructor<T> (Expression<T> lambda)
		{
			NewExpression	expression;

			expression = lambda.Body as NewExpression;

			if (expression == null)
				throw new ArgumentException ("can't get constructor information from expression", "lambda");

			return expression.Constructor;
		}

		public static FieldInfo Field<T> (Expression<T> lambda)
		{
			MemberExpression	expression;

			expression = lambda.Body as MemberExpression;

			if (expression == null || expression.Member.MemberType != MemberTypes.Field)
				throw new ArgumentException ("can't get field information from expression", "lambda");

			return (FieldInfo)expression.Member;
		}

		public static MethodInfo Method<T> (Expression<T> lambda)
		{
			MethodCallExpression	expression;

			expression = lambda.Body as MethodCallExpression;

			if (expression == null)
				throw new ArgumentException ("can't get method information from expression", "lambda");

			return expression.Method;
		}

		public static PropertyInfo Property<T> (Expression<T> lambda)
		{
			MemberExpression	expression;

			expression = lambda.Body as MemberExpression;

			if (expression == null || expression.Member.MemberType != MemberTypes.Property)
				throw new ArgumentException ("can't get property information from expression", "lambda");

			return (PropertyInfo)expression.Member;
		}
	}
}
