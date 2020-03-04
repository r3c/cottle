using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cottle.Documents.Emitted
{
    internal static class Resolver
    {
        public static ConstructorInfo Constructor<T>(Expression<T> lambda)
        {
            if (!(lambda.Body is NewExpression expression))
                throw new ArgumentException("can't get constructor information from expression", nameof(lambda));

            return expression.Constructor;
        }

        public static FieldInfo Field<T>(Expression<T> lambda)
        {
            if (!(lambda.Body is MemberExpression expression) || expression.Member.MemberType != MemberTypes.Field)
                throw new ArgumentException("can't get field information from expression", nameof(lambda));

            return (FieldInfo)expression.Member;
        }

        public static MethodInfo Method<T>(Expression<T> lambda)
        {
            if (!(lambda.Body is MethodCallExpression expression))
                throw new ArgumentException("can't get method information from expression", nameof(lambda));

            return expression.Method;
        }

        public static PropertyInfo Property<T>(Expression<T> lambda)
        {
            if (!(lambda.Body is MemberExpression expression) || expression.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException("can't get property information from expression", nameof(lambda));

            return (PropertyInfo)expression.Member;
        }
    }
}