using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceInterface.Model;
using ServiceLib.Service;

namespace FastPosFrontend.Configurations
{
    public class LoginHistory:IConfigurationProperty
    {
        [DataMember]
        public IList<User> Users { get; set; } = new List<User>();

        [DataMember]
        public long LastLoggedUserId { get; set; }
        public string LastLoggedUserByUsername { get; set; }

        public event EventHandler<SaveRequestedEventArgs> SaveRequested;

        public void RequestSave()
        {
            SaveRequested?.Invoke(this, new SaveRequestedEventArgs());
        }
    }
}
