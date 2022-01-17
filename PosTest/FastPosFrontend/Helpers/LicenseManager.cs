using Caliburn.Micro;
using FastPosFrontend.Configurations;
using Newtonsoft.Json;
using RestSharp;
using ServiceLib.Service;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FastPosFrontend.Helpers
{
    public class LicenseManager
    {
        public  readonly IRestClient Client;

        public  readonly IRestRequest _getLicenseState;
        public  readonly IRestRequest _doServerActivation;

        public LicenseManager(string url)
        {
            
            Client = new RestClient(url) { FollowRedirects = true};
            
            _getLicenseState = new RestRequest("/licensing/get-state", Method.GET);
            _doServerActivation = new RestRequest("/licensing/server-activation", Method.POST);
        }
       

        public void CheckLicenseState()
        {
            var res = Client.Execute(_getLicenseState);
            if ((int)res.StatusCode != 200)
            {
                List<string> errors = ParseErrorsFromResponse(res);

                if (errors.Contains(LicensingErrors.SERVER_ACTIVATION_REQUIRED))
                {
                    var result = PromptUserForCredentials();
                }

                if (errors.Contains(LicensingErrors.SERIAL_KEY_INVALID))
                {
                    NotifyUserOfErrorsAndShutdown(errors);
                }


            }

        }

        private static List<string> ParseErrorsFromResponse(IRestResponse res)
        {
            return JsonConvert.DeserializeObject<List<string>>(res.Content);
        }

        private bool PromptUserForCredentials()
        {
            var credentials = new LicensingCredentials()
            {
                Email = "example1@mail.com",
                Password = "sfghkbfj,mwerjsd"
            };

            List<string> errors = new List<string>();
            var result = ModalDialogBox.Submit(credentials, "LicenseManagerCredentialsDialogContent", "License Manager", o =>
            {
                return!string.IsNullOrWhiteSpace(credentials.Email) && !string.IsNullOrWhiteSpace(credentials.Password);
            }, onSubmit: () =>
              {
                var content = JsonConvert.SerializeObject(credentials);

                  _doServerActivation.AddJsonBody(content);
                var res = Client.Execute(_doServerActivation);

                  bool hasErrors = (int)res.StatusCode != 200;
                  if (hasErrors)
                  {
                      errors = ParseErrorsFromResponse(res);
                  }
                return !hasErrors ;
            }).Show();


            if (!result)
            {
                NotifyUserOfErrorsAndShutdown(errors);
            }

            return result;
            
        }

        private static void NotifyUserOfErrorsAndShutdown(List<string> errors)
        {
            var formattedErrorsMessage = string.Join("\n", errors.Select(error => $"* {error}"));
            ModalDialogBox.Ok(formattedErrorsMessage, "License Manager").Show();
            Application.Current.Shutdown();
        }





    }

    public class LicensingCredentials: PropertyChangedBase
    {
        private string _email;
        private string _password;
        [JsonProperty("email")]
        public string Email
        {
            get { return _email; }
            set { Set(ref _email , value); }
        }

        [JsonProperty("password")]
        public string Password
        {
            get { return _password; }
            set { Set(ref _password , value); }
        }


    }

    public class LicensingErrors
    {
        public const string CREDENTIALS_MISSING = "com.softlines.errors.licensing.credentials.Missing";
        public const string CREDENTIALS_FORMAT = "com.softlines.errors.licensing.credentials.Format";
        public const string CREDENTIALS_INVALID = "com.softlines.errors.licensing.credentials.Invalid";

        public const string SERIAL_KEY_INVALID = "com.softlines.errors.licensing.key.Invalid";
        public const string SERIAL_KEY_FAKE = "com.softlines.errors.licensing.key.Fake";
        public const string SERIAL_KEY_BLACKLISTED = "com.softlines.errors.licensing.key.Blacklisted";

        public const string LICENSE_ACTIVATION_REACTIVATION = "com.softlines.errors.licensing.activation.ReactivationBeforeInit";
        public const string LICENSE_ACTIVATION_HWD_ID_MISMATCH = "com.softlines.errors.licensing.activation.HardwareIdMismatch";
        public const string LICENSE_ACTIVATION_EXPIRED = "com.softlines.errors.licensing.activation.ExpiredLicense";
        public const string LICENSE_ACTIVATION_MAX_ACTIVATIONS = "com.softlines.errors.licensing.activation.MaxActivationsReached";
        public const string SERVER_ACTIVATION_REQUIRED = "com.softlines.errors.licensing.ServerActivationRequired";
    }
}
