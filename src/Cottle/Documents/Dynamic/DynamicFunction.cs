using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Cottle.Values;

namespace Cottle.Documents.Dynamic
{
    internal class DynamicFunction : IFunction
    {
        public bool IsPure => false;

        private readonly DynamicRenderer _renderer;

        private readonly DynamicStorage _storage;

        public DynamicFunction(IEnumerable<string> arguments, Command command)
        {
            var method = new DynamicMethod(string.Empty, typeof(Value),
                new[] { typeof(DynamicStorage), typeof(IStore), typeof(IReadOnlyList<Value>), typeof(TextWriter) },
                GetType());
            var compiler = new DynamicCompiler(method.GetILGenerator());
            var storage = compiler.Compile(arguments, command);

            _renderer = (DynamicRenderer)method.CreateDelegate(typeof(DynamicRenderer));
            _storage = storage;
        }

        public static void Save(Command command, Trimmer trimmer, string assemblyName, string fileName)
        {
#if NETSTANDARD2_0
            var assembly =
                AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule(fileName);
#else
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName),
                AssemblyBuilderAccess.RunAndSave);
            var module = assembly.DefineDynamicModule(assemblyName, fileName);
#endif

            var program = module.DefineType("Program", TypeAttributes.Public);
            var method = program.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static, typeof(Value),
                new[] { typeof(DynamicStorage), typeof(IList<Value>), typeof(IStore), typeof(TextWriter) });

            var compiler = new DynamicCompiler(method.GetILGenerator());
            compiler.Compile(Enumerable.Empty<string>(), command);

#if NETSTANDARD2_0
            program.CreateTypeInfo();
#else
            program.CreateType();
            assembly.Save(fileName);
#endif
        }

        public int CompareTo(IFunction other)
        {
            return object.ReferenceEquals(this, other) ? 0 : 1;
        }

        public bool Equals(IFunction other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is IFunction other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _renderer.GetHashCode();
        }

        public Value Invoke(object state, IReadOnlyList<Value> arguments, TextWriter output)
        {
            if (!(state is IStore store))
                throw new InvalidOperationException($"Invalid function invoke, you seem to have injected a function declared in a {nameof(DynamicDocument)} from another type of document.");

            return _renderer(_storage, store, arguments, output);
        }

        public override string ToString()
        {
            return "<dynamic>";
        }
    }
}