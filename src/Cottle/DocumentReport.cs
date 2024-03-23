using System;

namespace Cottle
{
    public readonly struct DocumentReport
    {
        public readonly int Length;
        public readonly DocumentReportLevel Level;
        public readonly string Message;
        public readonly int Offset;
        public readonly DocumentSeverity Severity;
        public readonly DocumentReportType Type;

        public DocumentReport(DocumentReportType type, DocumentReportLevel level, int offset, int length,
            string message)
        {
            Length = length;
            Level = level;
            Message = message;
            Offset = offset;
            Severity = level switch
            {
                DocumentReportLevel.Notice => DocumentSeverity.Notice,
                DocumentReportLevel.Warning => DocumentSeverity.Warning,
                _ => DocumentSeverity.Error
            };
            Type = type;
        }

        [Obsolete("Provide `type` and `level` arguments as replacement for `severity`")]
        public DocumentReport(DocumentSeverity severity, int offset, int length, string message)
        {
            Length = length;
            Level = severity switch
            {
                DocumentSeverity.Notice => DocumentReportLevel.Notice,
                DocumentSeverity.Warning => DocumentReportLevel.Warning,
                _ => DocumentReportLevel.Error
            };
            Message = message;
            Offset = offset;
            Severity = severity;
            Type = DocumentReportType.Language;
        }
    }
}