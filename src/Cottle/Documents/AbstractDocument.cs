using System.Globalization;
using System.IO;

namespace Cottle.Documents
{
    public abstract class AbstractDocument : IDocument
    {
        public abstract Value Render(IContext context, TextWriter writer);

        public string Render(IContext context)
        {
            var writer = new StringWriter(CultureInfo.InvariantCulture);

            Render(context, writer);

            return writer.ToString();
        }

        internal static DocumentConfiguration CreateConfiguration(ISetting setting)
        {
            var trimmer = setting.Trimmer;

            return new DocumentConfiguration
            {
                BlockBegin = setting.BlockBegin,
                BlockContinue = setting.BlockContinue,
                BlockEnd = setting.BlockEnd,
                Escape = setting.Escape,
                NoOptimize = !setting.Optimize,
                Trimmer = s => trimmer(s)
            };
        }
    }
}