namespace Mappy.Maybe
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class MaybeWasNoneException : Exception
    {
        public MaybeWasNoneException()
        {
        }

        public MaybeWasNoneException(string message)
            : base(message)
        {
        }

        public MaybeWasNoneException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MaybeWasNoneException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}