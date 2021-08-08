using System;
using System.Collections.Generic;

namespace ServiceLib.Exceptions
{
    public class StateNotManagedException<TState> : Exception
    {
        private static  string _message = $"Trying to access unmanaged state of type {typeof(TState)}";
        public StateNotManagedException():base(_message, new KeyNotFoundException()) { }
        

        public StateNotManagedException(Exception innerException) : base(_message, innerException) { }


    }
}
