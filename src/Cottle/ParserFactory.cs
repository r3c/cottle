using Cottle.Parsers;

namespace Cottle
{
    internal static class ParserFactory
    {
        public static IParser BuildParser(DocumentConfiguration configuration)
        {
            var blockBegin = configuration.BlockBegin ?? DocumentConfiguration.DefaultBlockBegin;
            var blockContinue = configuration.BlockContinue ?? DocumentConfiguration.DefaultBlockContinue;
            var blockEnd = configuration.BlockEnd ?? DocumentConfiguration.DefaultBlockEnd;
            var escape = configuration.Escape.GetValueOrDefault(DocumentConfiguration.DefaultEscape);
            var trimmer = configuration.Trimmer ?? DocumentConfiguration.TrimIndentCharacters;

            IParser parser = new ForwardParser(blockBegin, blockContinue, blockEnd, escape, trimmer);

            return configuration.NoOptimize ? parser : new OptimizeParser(parser);
        }
    }
}