using System.Collections.Generic;

namespace Cottle
{
    internal class ParserState
    {
        public IReadOnlyList<DocumentReport> Reports => _reports;

        private readonly List<DocumentReport> _reports = new List<DocumentReport>();

        public void AddReport(DocumentReport report)
        {
            _reports.Add(report);
        }
    }
}