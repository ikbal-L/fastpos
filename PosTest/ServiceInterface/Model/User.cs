using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    [DataContract]
    public class User
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string PinCode { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        public List<Role> Roles { get; set; }

        [DataMember]
        public List<long> RoleIds { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        [JsonConverter(typeof(StringEnumConverter))]
        public Descriptor Descriptor { get; set; } = Descriptor.User;
    }

    [DataContract]
    public class Waiter : User
    {
 
    }
    
    [DataContract]
    public class Delivereyman : User
    {

    }

    [DataContract]
    public class Role
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public List<Permission> Permissions { get; set; }
    }

    [DataContract]
    public class Permission
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string[] Permissions { get; set; }

    }

}

namespace ServiceInterface
{
    public enum Descriptor
    {
        User,
        Server,
        Deliverymen
    }
}