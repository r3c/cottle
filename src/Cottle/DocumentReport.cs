namespace Cottle
{
    public readonly struct DocumentReport
    {
        public readonly int Length;
        public readonly string Message;
        public readonly int Offset;
        public readonly DocumentSeverity Severity;

        public DocumentReport(DocumentSeverity severity, int offset, int length, string message)
        {
            Length = length;
            Message = message;
            Offset = offset;
            Severity = severity;
        }
    }
}