using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cottle.Documents.Emitted
{
    internal static class Resolver
    {
        public static ConstructorInfo Constructor<T>(Expression<T> lambda)
        {
            if (lambda.Body is not NewExpression expression || expression.Constructor == null)
                throw new ArgumentException("can't get constructor information from expression", nameof(lambda));

            return expression.Constructor;
        }

        public static FieldInfo Field<T>(Expression<T> lambda)
        {
            if (lambda.Body is not MemberExpression expression || expression.Member is not FieldInfo fieldInfo)
                throw new ArgumentException("can't get field information from expression", nameof(lambda));

            return fieldInfo;
        }

        public static MethodInfo Method<T>(Expression<T> lambda)
        {
            if (lambda.Body is not MethodCallExpression expression)
                throw new ArgumentException("can't get method information from expression", nameof(lambda));

            return expression.Method;
        }

        public static PropertyInfo Property<T>(Expression<T> lambda)
        {
            if (lambda.Body is not MemberExpression expression || expression.Member is not PropertyInfo propertyInfo)
                throw new ArgumentException("can't get property information from expression", nameof(lambda));

            return propertyInfo;
        }
    }
}