using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IResponseHandler
    {
        void Handle<T>(int status, IEnumerable<string> errors, StateManagementQuery query, string source = "",
            bool overrideDefaultBehaviour = false, T obj = null) where T : class;
    }
    public enum StateManagementQuery
    {
        Save,
        Update,
        Delete,
        Retrieve
    }
}