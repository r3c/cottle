using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cottle.Documents.Dynamic
{
	static class Resolver
	{
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
