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