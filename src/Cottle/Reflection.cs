using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cottle
{
    internal static class Reflection
    {
        public static MethodInfo ChangeGenericDeclaringType(MethodInfo method, params Type[] types)
        {
            var declaringType = method.DeclaringType ?? throw new ArgumentOutOfRangeException(nameof(method), "method has no declaring type");
            var newDeclaringType = declaringType.GetGenericTypeDefinition().MakeGenericType(types);
            var newMethod = MethodBase.GetMethodFromHandle(method.MethodHandle, newDeclaringType.TypeHandle);
            var result = newMethod as MethodInfo ?? throw new InvalidOperationException("invalid method after changing declaring type");

            return result;
        }

        public static ConstructorInfo GetConstructor<T>(Expression<T> lambda)
        {
            if (lambda.Body is not NewExpression expression || expression.Constructor is null)
                throw new ArgumentException("can't get constructor information from expression", nameof(lambda));

            return expression.Constructor;
        }

        public static FieldInfo GetField<T>(Expression<T> lambda)
        {
            if (lambda.Body is not MemberExpression expression || expression.Member is not FieldInfo fieldInfo)
                throw new ArgumentException("can't get field information from expression", nameof(lambda));

            return fieldInfo;
        }

        public static MethodInfo GetMethod<T>(Expression<T> lambda)
        {
            if (lambda.Body is not MethodCallExpression expression)
                throw new ArgumentException("can't get method information from expression", nameof(lambda));

            return expression.Method;
        }

        public static PropertyInfo GetProperty<T>(Expression<T> lambda)
        {
            if (lambda.Body is not MemberExpression expression || expression.Member is not PropertyInfo propertyInfo)
                throw new ArgumentException("can't get property information from expression", nameof(lambda));

            return propertyInfo;
        }
    }
}