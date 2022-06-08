//#define COTTLE_IL_SAVE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted
{
    internal readonly struct Program
    {
        private static readonly MethodInfo ExecutableInvoke =
            Reflection.GetMethod<Func<Execute, bool>>(e => e.Invoke(Array.Empty<Value>(), new Frame(Array.Empty<Value>(), Array.Empty<Value>(), default), TextWriter.Null, out Program._unused));

        private static readonly Type[] ExecutableInvokeArguments =
            Program.ExecutableInvoke.GetParameters().Select(p => p.ParameterType).ToArray();

        private static Value _unused;

        public static Program Create(IStatementGenerator generator, IReadOnlyList<Symbol> arguments)
        {
            var dynamicMethod = new DynamicMethod(string.Empty, Program.ExecutableInvoke.ReturnType,
                Program.ExecutableInvokeArguments, typeof(EmittedDocument));
            var emitter = new Emitter(dynamicMethod.GetILGenerator());

            Program.Emit(emitter, generator, arguments);

#if COTTLE_IL_SAVE && NET472
            var directory = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            Program.Save(generator, Path.Combine(directory, "Cottle.GeneratedIL.dll"));
#endif

            var executable = (Execute)dynamicMethod.CreateDelegate(typeof(Execute));

            return new Program(executable, emitter.CreateConstants());
        }

        private static void Emit(Emitter emitter, IStatementGenerator generator, IReadOnlyList<Symbol> arguments)
        {
            var body = emitter.DeclareLabel();

            // Load function arguments to locals
            for (var i = 0; i < arguments.Count; ++i)
            {
                emitter.EmitLoadInteger(i);
                emitter.EmitLoadFrameArgumentLength();
                emitter.EmitBranchWhenGreaterOrEqual(body);
                emitter.EmitLoadFrameArgument(i);
                emitter.EmitStoreLocal(emitter.GetOrDeclareLocal(arguments[i]));
            }

            // Execute function body and return
            emitter.MarkLabel(body);

            if (!generator.Generate(emitter))
                emitter.EmitLoadBoolean(false);

            emitter.EmitReturn();
        }

#if COTTLE_IL_SAVE && NET472
        private static void Save(IStatementGenerator generator, string filePath)
        {
            var assemblyName = new AssemblyName("Test");
            var fileName = Path.GetFileName(filePath);

            var saveAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var saveModule = saveAssembly.DefineDynamicModule(assemblyName.Name, fileName);
            var saveProgram = saveModule.DefineType("Program", TypeAttributes.Public);
            var saveMethod = saveProgram.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Any, Program.ExecutableInvoke.ReturnType, Program.ExecutableInvokeArguments);
            var saveEmitter = new Emitter(saveMethod.GetILGenerator());

            Program.Emit(saveEmitter, generator, Array.Empty<Symbol>());

            saveProgram.CreateType();
            saveAssembly.Save(fileName);

            var saveSource = Path.Combine(Environment.CurrentDirectory, fileName);

            File.Copy(saveSource, filePath, true);
        }
#endif

        private readonly IReadOnlyList<Value> _constants;
        private readonly Execute _execute;

        private Program(Execute execute, IReadOnlyList<Value> constants)
        {
            _constants = constants;
            _execute = execute;
        }

        public Value Execute(Frame frame, TextWriter output)
        {
            return _execute(_constants, frame, output, out var result)
                ? result
                : Value.Undefined;
        }
    }
}