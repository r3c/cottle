using System;
using System.Reflection.Emit;

namespace Cottle
{
    internal class DynamicMethodCreator<TDelegate> where TDelegate : Delegate
    {
        private static readonly Type DelegateType = typeof(TDelegate);

        public ILGenerator Generator => _dynamicMethod.GetILGenerator();

        private readonly DynamicMethod _dynamicMethod;

        public DynamicMethodCreator()
        {
            var invoke = DelegateType.GetMethod("Invoke");

            if (invoke is null)
                throw new ArgumentOutOfRangeException(nameof(TDelegate), DelegateType, "generic parameter must be a delegate type");

            var parameterTypes = Array.ConvertAll(invoke.GetParameters(), p => p.ParameterType);
            var returnType = invoke.ReturnType;

            _dynamicMethod = new DynamicMethod(string.Empty, returnType, parameterTypes, typeof(Dynamic).Module, true);
        }

        public TDelegate Create()
        {
            return (TDelegate)_dynamicMethod.CreateDelegate(DelegateType);
        }
    }
}