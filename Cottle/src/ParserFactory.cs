using Cottle.Parsers;
using Cottle.Parsers.Post;
using Cottle.Parsers.Post.Optimizers;

namespace Cottle
{
    internal static class ParserFactory
    {
        public static IParser BuildParser(ISetting setting)
        {
            IParser parser = new ForwardParser(setting.BlockBegin, setting.BlockContinue, setting.BlockEnd,
                setting.Escape);

            if (setting.Optimize)
                parser = new PostParser(parser, new IOptimizer[]
                {
                    ConstantInvokeOptimizer.Instance,
                    ConstantMapOptimizer.Instance,
                    IfOptimizer.Instance,
                    ReturnOptimizer.Instance
                });

            return parser;
        }
    }
}