using Cottle.Parsers;

namespace Cottle
{
    internal static class Parser
    {
        public static IParser Create(DocumentConfiguration configuration)
        {
            var blockBegin = configuration.BlockBegin ?? DocumentConfiguration.DefaultBlockBegin;
            var blockContinue = configuration.BlockContinue ?? DocumentConfiguration.DefaultBlockContinue;
            var blockEnd = configuration.BlockEnd ?? DocumentConfiguration.DefaultBlockEnd;
            var escape = configuration.Escape.GetValueOrDefault(DocumentConfiguration.DefaultEscape);
            var trimmer = configuration.Trimmer ?? DocumentConfiguration.TrimFirstAndLastBlankLines;

            IParser parser = new ForwardParser(blockBegin, blockContinue, blockEnd, escape, trimmer);

            return configuration.NoOptimize ? parser : new OptimizeParser(parser);
        }
    }
}