namespace TAUtil
{
    public class TdfParseException : ParseException
    {
        public TdfParseException(string message, int lineNumber, int columnNumber)
            : base(message)
        {
            this.LineNumber = lineNumber;
            this.ColumnNumber = columnNumber;
        }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }
    }
}
