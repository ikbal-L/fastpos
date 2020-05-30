using System;
using System.Runtime.Serialization;

namespace ServiceInterface.Model
{
    [Serializable]
    internal class ProductMappingException : Exception
    {
        public ProductMappingException()
        {
        }

        public ProductMappingException(string message) : base(message)
        {
        }

        public ProductMappingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProductMappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}