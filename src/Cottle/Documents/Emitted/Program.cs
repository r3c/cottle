// #define COTTLE_IL_SAVE

using System.Collections.Generic;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted
{
    internal readonly struct Program
    {
        public static Program Create(IStatementGenerator generator, IReadOnlyList<Symbol> arguments)
        {
            var creator = Dynamic.DefineMethod<Execute>();
            var emitter = new Emitter(creator.Generator);

            Program.Emit(emitter, generator, arguments);

#if COTTLE_IL_SAVE && NET472
            var directory = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            Program.Save(generator, Path.Combine(directory, "Cottle.GeneratedIL.dll"));
#endif

            var executable = creator.Create();

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
            var assemblyName = new System.Reflection.AssemblyName("Test");
            var fileName = Path.GetFileName(filePath);

            var saveAssembly = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
            var saveModule = saveAssembly.DefineDynamicModule(assemblyName.Name, fileName);
            var saveProgram = saveModule.DefineType("Program", System.Reflection.TypeAttributes.Public);
            var saveMethod = saveProgram.DefineMethod("Main", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static,
                System.Reflection.CallingConventions.Any, typeof(bool), new[] { typeof(IReadOnlyList<Value>), typeof(Frame), typeof(TextWriter), typeof(Value).MakeByRefType() });
            var saveEmitter = new Emitter(saveMethod.GetILGenerator());

            Program.Emit(saveEmitter, generator, System.Array.Empty<Symbol>());

            saveProgram.CreateType();
            saveAssembly.Save(fileName);

            var saveSource = Path.Combine(System.Environment.CurrentDirectory, fileName);

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