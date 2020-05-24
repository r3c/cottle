using System.IO;
using System.Text;

namespace Cottle.Demo.Serialization
{
    internal static class StateSerializer
    {
        private const int CurrentVersion = 3;
        private const int MinimalVersion = 1;

        public static bool TryRead(Stream stream, out State state)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                var version = reader.ReadInt32();

                if (version < StateSerializer.MinimalVersion || version > StateSerializer.CurrentVersion)
                {
                    state = default;

                    return false;
                }

                if (!ValueSerializer.TryRead(reader, version, out var values))
                {
                    state = default;

                    return false;
                }

                var blockBegin = reader.ReadString();
                var blockContinue = reader.ReadString();
                var blockEnd = reader.ReadString();

                int trimmerIndex;

                if (version > 1)
                    trimmerIndex = reader.ReadInt32();
                else
                    trimmerIndex = TrimmerSerializer.DefaultIndex;

                var configuration = new DocumentConfiguration
                {
                    BlockBegin = blockBegin,
                    BlockContinue = blockContinue,
                    BlockEnd = blockEnd,
                    Trimmer = TrimmerSerializer.GetFunction(trimmerIndex)
                };

                var template = reader.ReadString();

                state = new State(configuration, values, template);

                return true;
            }
        }

        public static bool TryWrite(Stream stream, State state)
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                writer.Write(StateSerializer.CurrentVersion);

                if (!ValueSerializer.TryWrite(writer, state.Values))
                    return false;

                writer.Write(state.Configuration.BlockBegin);
                writer.Write(state.Configuration.BlockContinue);
                writer.Write(state.Configuration.BlockEnd);
                writer.Write(TrimmerSerializer.GetIndex(state.Configuration.Trimmer));
                writer.Write(state.Template);
            }

            return true;
        }
    }
}