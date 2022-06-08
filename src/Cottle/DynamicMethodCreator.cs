//#define COTTLE_IL_SAVE

using System;
using System.Reflection.Emit;

namespace Cottle
{
    internal class DynamicMethodCreator<TDelegate> where TDelegate : Delegate
    {
        private static readonly Type DelegateType = typeof(TDelegate);

        public ILGenerator Generator => _generator;

        private readonly Func<TDelegate> _constructor;
        private readonly ILGenerator _generator;

        public DynamicMethodCreator()
        {
            var invoke = DelegateType.GetMethod("Invoke");

            if (invoke is null)
                throw new ArgumentOutOfRangeException(nameof(TDelegate), DelegateType, "generic parameter must be a delegate type");

            var parameterTypes = Array.ConvertAll(invoke.GetParameters(), p => p.ParameterType);
            var returnType = invoke.ReturnType;

#if COTTLE_IL_SAVE && NET472
            var assemblyName = new System.Reflection.AssemblyName("Cottle.Debug");
            var directoryName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var fileName = assemblyName.Name + ".dll";

            var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var module = assembly.DefineDynamicModule(assemblyName.Name, fileName);
            var program = module.DefineType("Program", System.Reflection.TypeAttributes.Public);
            var method = program.DefineMethod("Main", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static, System.Reflection.CallingConventions.Any, returnType, parameterTypes);

            _constructor = () =>
            {
                var previousDirectory = Environment.CurrentDirectory;

                try
                {
                    Environment.CurrentDirectory = directoryName;

                    program.CreateType();
                    assembly.Save(fileName);

                    return (TDelegate)method.CreateDelegate(DelegateType);
                }
                finally
                {
                    Environment.CurrentDirectory = previousDirectory;
                }
            };

            _generator = method.GetILGenerator();
#else
            var method = new DynamicMethod(string.Empty, returnType, parameterTypes, typeof(Dynamic).Module, true);

            _constructor = () => (TDelegate)method.CreateDelegate(DelegateType);
            _generator = method.GetILGenerator();
#endif
        }

        public TDelegate Create()
        {
            return _constructor();
        }
    }
}