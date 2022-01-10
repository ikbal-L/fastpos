using Caliburn.Micro;
using FastPosFrontend.Configurations;
using Newtonsoft.Json;
using RestSharp;
using ServiceLib.Service;
using System.Collections.Generic;

namespace FastPosFrontend.Helpers
{
    public class LicenseManager
    {
        public static readonly IRestClient Client;

        public static readonly IRestRequest _licenseStatusCheck;
        public static readonly IRestRequest _submitLicensingCredentials;

        static LicenseManager()
        {
            var baseUrl = ConfigurationManager.Get<PosConfig>().Url;
            Client = new RestClient(baseUrl);
            _licenseStatusCheck = new RestRequest("/", Method.GET);
            _submitLicensingCredentials = new RestRequest("/", Method.POST);
        }
       

        public void Check()
        {
            var res = Client.Execute(_licenseStatusCheck);
            if ((int)res.StatusCode == 445)
            {
                var errors = JsonConvert.DeserializeObject<List<string>>(res.Content);
    
                if (errors.Contains(LicensingErrors.CREDENTIALS_MISSING))
                {
                    var result =  PromptUserForCredentials();
                }
            }
           
        }

        private bool PromptUserForCredentials()
        {
            //var credentials = new LicensingCredentials()
            //{
            //    Email = "example1@mail.com",
            //    Password = "sfghkbfj,mwerjsd"
            //};

            var credentials = new LicensingCredentials()
            {
                Email = "example@mail.com",
                Password = ""
            };

            var result  = ModalDialogBox.Submit(credentials, "LicenseManagerCredentialsDialogContent","License Manager", o =>
            {
                return!string.IsNullOrWhiteSpace(credentials.Email) && !string.IsNullOrWhiteSpace(credentials.Password);
            },onSubmit:()=> 
            {
                var content = JsonConvert.SerializeObject(credentials);
                _submitLicensingCredentials.AddHeader("Licensing-Credentials", content);
                var res = Client.Execute(_submitLicensingCredentials);

                return (int)res.StatusCode != 445;
            }).Show();


            if (!result)
            {
                ModalDialogBox.Ok("licensing Failed ", "License Manager").Show();
                App.Current.Shutdown();
            }

            return result;
            
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
    }
}
