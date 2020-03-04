using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted
{
    internal readonly struct Program
    {
        private static readonly Type OutValue = typeof(Value).MakeByRefType();

        public static Program Create(IGenerator generator)
        {
            var arguments = new[] { typeof(IReadOnlyList<Value>), typeof(Frame), typeof(TextWriter), Program.OutValue };
            var method = new DynamicMethod(string.Empty, typeof(bool), arguments, typeof(EmittedDocument));
            var emitter = new Emitter(method.GetILGenerator());

            generator.Generate(emitter);
            emitter.Return();

            var executable = (Executable)method.CreateDelegate(typeof(Executable));

            return new Program(executable, emitter.CreateConstants());
        }

        public readonly IReadOnlyList<Value> Constants;
        public readonly Executable Executable;

        private Program(Executable executable, IReadOnlyList<Value> constants)
        {
            Constants = constants;
            Executable = executable;
        }
    }
}