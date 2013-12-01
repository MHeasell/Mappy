namespace TAUtil
{
    using System;

    /// <summary>
    /// The exception that is thrown when a reader is unable to parse input,
    /// usually because of errors in the input.
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException()
        {
        }

        public ParseException(string message)
            : base(message)
        {
        }
    }
}
