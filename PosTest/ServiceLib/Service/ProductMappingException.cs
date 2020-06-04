﻿using System;
using System.Runtime.Serialization;

namespace ServiceLib.Service
{
    [Serializable]
    internal class MappingException : Exception
    {
        public MappingException()
        {
        }

        public MappingException(string message) : base(message)
        {
        }

        public MappingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
