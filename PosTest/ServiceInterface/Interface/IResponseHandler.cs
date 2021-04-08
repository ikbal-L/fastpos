using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface IResponseHandler
    {
        void Handle<TState>(int status,IEnumerable<string> errors,StateManagementAction action,string source= "", bool overrideDefaultBehaviour = false);
    }
    public enum StateManagementAction
    {
        Save,
        Update,
        Delete,
        Retrieve
    }
}