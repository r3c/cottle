namespace Cottle
{
    public readonly struct DocumentReport
    {
        public readonly int Column;
        public readonly int Line;
        public readonly string Message;

        public DocumentReport(string message, int line, int column)
        {
            Column = column;
            Line = line;
            Message = message;
        }
    }
}