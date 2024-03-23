using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Cottle.Exceptions;

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

        internal static ParseException CreateException(IEnumerable<DocumentReport> reports)
        {
            var firstError = reports.FirstOrDefault(r => r.Level == DocumentReportLevel.Error);

            throw new ParseException(firstError.Offset, firstError.Length, firstError.Message ?? "unknown error");
        }
    }
}