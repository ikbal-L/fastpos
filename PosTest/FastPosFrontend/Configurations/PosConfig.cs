
using Caliburn.Micro;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Configurations
{
    public class PosConfig : PropertyChangedBase, IConfiguration
    {
        public const string FILE_NAME = "pos.appconfig";
        private GeneralSettings _general = new GeneralSettings();

        [DataMember]
        public GeneralSettings General
        {
            get => _general; 
            set
            {
                _general.SaveRequested-= General_SaveRequested;
                _general = value;
                _general.SaveRequested += General_SaveRequested;
            }
        }
        [DataMember]
        public Printing Printing { get; set; } = new Printing();
        [DataMember]
        public LoginHistory LoginHistory { get; set; } = new LoginHistory();
        [DataMember]
        public ProductLayoutConfiguration ProductLayout { get; set; } = new ();

        [DataMember]
        public CategoryLayoutConfiguration CategoryLayout { get; set; } = new();

        [DataMember]
        public AdditiveLayoutConfiguration AdditiveLayout { get; set; } = new();

        [DataMember]
        public string Url { get; set; } = "http://localhost:8080";

        public event EventHandler<SaveRequestedEventArgs> SaveRequested;

        public PosConfig()
        {
            
            LoginHistory.SaveRequested += LoginHistory_SaveRequested;
            ProductLayout.SaveRequested += ProductLayout_SaveRequested;
            CategoryLayout.SaveRequested += CategoryLayout_SaveRequested;
            AdditiveLayout.SaveRequested += AdditiveLayout_SaveRequested;
            Printing.SaveRequested += Printing_SaveRequested;

        }

        private void AdditiveLayout_SaveRequested(object sender, SaveRequestedEventArgs e)
        {
            ForwardRequest(sender);
        }

        private void CategoryLayout_SaveRequested(object sender, SaveRequestedEventArgs e)
        {
            ForwardRequest(sender);
        }

        private void Printing_SaveRequested(object sender, SaveRequestedEventArgs e)
        {
            ForwardRequest(sender);
        }

        private void ProductLayout_SaveRequested(object sender, SaveRequestedEventArgs e)
        {
            ForwardRequest(sender);
        }

        private void LoginHistory_SaveRequested(object sender, SaveRequestedEventArgs e)
        {
            ForwardRequest(sender);
        }

        private void General_SaveRequested(object sender, SaveRequestedEventArgs e)
        {
            ForwardRequest(sender);
        }

        public void ForwardRequest(object source)
        {
            SaveRequested?.Invoke(this, new SaveRequestedEventArgs() { OriginalSource = source });
        }


    }
}
