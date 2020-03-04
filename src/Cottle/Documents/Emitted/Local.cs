using System.Reflection.Emit;

namespace Cottle.Documents.Emitted
{
    internal readonly struct Local<TValue>
    {
        public readonly LocalBuilder Builder;

        public Local(LocalBuilder builder)
        {
            Builder = builder;
        }
    }
}