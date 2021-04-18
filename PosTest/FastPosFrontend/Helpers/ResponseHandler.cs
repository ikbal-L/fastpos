﻿using System.Collections;
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
        public void Handle<T>(int status,IEnumerable<string> errors,StateManagementAction action,string source = "",bool overrideDefaultBehaviour = false,string identifier = "",T obj = null) where T:class
        {
            var type = typeof(T);
            object entityIdentifier = null;
            if (!string.IsNullOrEmpty(identifier)&& obj!= null)
            {
                entityIdentifier =type.GetProperty(identifier)?.GetValue(obj);
            }
            var successMessage = obj == null? $"{action}d {type.Name}(s) Successfully": $"{action}d {type.Name} {entityIdentifier} Successfully"
                ;
            var errorsString = JsonConvert.SerializeObject(errors, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            });
            var errorMessage = $" [{(HttpStatusCode)status}]: Unable to {action} {type.Name}(s)";
            switch (status)
            {
                case 200:
                case 201:

                    if (action != StateManagementAction.Retrieve || overrideDefaultBehaviour)
                    {
                        ToastNotification.Notify(successMessage, NotificationType.Success); 
                    }
                    break;
                case 204:
                    ToastNotification.Notify($"No {type.Name}s to {action}",NotificationType.Information);
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