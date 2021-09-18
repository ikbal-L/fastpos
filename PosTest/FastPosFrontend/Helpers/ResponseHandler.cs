using System.Collections.Generic;
using System.Net;
using System.Windows;
using Newtonsoft.Json;
using ServiceInterface.Interface;

namespace FastPosFrontend.Helpers
{
    public class ResponseHandler : IResponseHandler
    {
        static ResponseHandler()
        {
            Handler = new ResponseHandler();
        }

        public static IResponseHandler Handler { get; }

        public void Handle<T>(int status, IEnumerable<string> errors, StateManagementQuery action, string source = "",
            bool overrideDefaultBehaviour = false, T obj = null) where T : class
        {
            var type = typeof(T);

            var successMessage = obj == null
                    ? $"{action}d {type.Name}(s) Successfully"
                    : $"{action}d {type.Name} {obj} Successfully"
                ;
            var errorsString = JsonConvert.SerializeObject(errors, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            });
            var errorMessage = obj == null
                ? $" [{(HttpStatusCode) status}]: Unable to {action} {type.Name}(s)"
                : $" [{(HttpStatusCode) status}]: Unable to {action} {type.Name} {obj}";
            ;
            switch (status)
            {

                case 200:
                case 201:

                    //if (action != StateManagementAction.Retrieve || overrideDefaultBehaviour)
                    //{
                    //    ToastNotification.Notify(successMessage, NotificationType.Success);
                    //}

                    break;

                case 204:
                    ToastNotification.Notify($"No {type.Name}s to {action}", NotificationType.Information);
                    break;
                case 409:
                    ToastNotification.Notify($"The {type.Name} you added already exists");
                    break;
                case 422:

                    errorMessage += $" due to the following Errors:\n{errorsString}";
                    ToastNotification.Notify(errorMessage);
                    Clipboard.SetText(errorMessage);

                    break;
                case 404:
                    ToastNotification.Notify(errorMessage);
                    break;
                default:
                    ToastNotification.Notify($"[Error: {(HttpStatusCode) status}] Something happened");
                    break;
            }
        }
    }
}