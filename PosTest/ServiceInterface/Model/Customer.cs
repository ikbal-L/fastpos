using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ServiceInterface.Model
{
    [DataContract]
    public class Customer : PropertyChangedBase,IState<long>
    {
        private string _name;
        private long? _id;
        private string _mobile;

        [DataMember]
        public long? Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        [DataMember]
        [Required(ErrorMessage = "Customer Name must not be null or empty", AllowEmptyStrings = false)]
        [RegularExpression(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z-0-9_\s]*$", ErrorMessage = "Use Latin, Arabic or Numeric Characters only ")]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        [DataMember]
        //[Phone(ErrorMessage = "Enter a valid phone number ")]
        [Required(AllowEmptyStrings = false)]
        [MinLength(09,ErrorMessage = "Phone number length must be at least 9 digits ")]
        public string Mobile
        {
            get => _mobile;
            set => Set(ref _mobile, value);
        }

        public BindableCollection<String> PhoneNumbers { get; set; }    
    }    
}
