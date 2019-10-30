using Cottle.Parsers;

namespace Cottle
{
    internal static class ParserFactory
    {
        public static IParser BuildParser(ISetting setting)
        {
            IParser parser = new ForwardParser(setting.BlockBegin, setting.BlockContinue, setting.BlockEnd,
                setting.Escape, setting.Trimmer);

            return setting.Optimize ? new OptimizeParser(parser) : parser;
        }
    }
}