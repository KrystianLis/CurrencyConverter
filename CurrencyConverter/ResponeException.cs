using System;
using System.Runtime.Serialization;

namespace CurrencyConverter
{
    [Serializable]
    internal class ResponeException : Exception
    {
        public int StatusCode { get; set; }
        public string Content { get; set; }

        public ResponeException()
        {
        }

        public ResponeException(string message) : base(message)
        {
        }

        public ResponeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResponeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}