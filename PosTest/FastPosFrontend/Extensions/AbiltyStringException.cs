using System;
using System.Runtime.Serialization;

namespace FastPosFrontend.Extensions
{
    [Serializable]
    internal class AbiltyStringException : Exception
    {
        public AbiltyStringException()
        {
        }

        public AbiltyStringException(string message) : base(message)
        {
        }

        public AbiltyStringException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AbiltyStringException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}