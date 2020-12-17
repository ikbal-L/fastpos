using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Customer : IState<long>
    {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        [Required(ErrorMessage = "Customer Name must not be null or empty",AllowEmptyStrings = false)]
        public string Name { get; set; }

        [DataMember]
        [Phone(ErrorMessage = "Enter a valid phone number ")]
        public string Mobile { get; set; }
    }    
}
