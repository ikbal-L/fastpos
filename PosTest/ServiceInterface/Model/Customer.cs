using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        [DataMember]
        public long? Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        [DataMember]
        [RegularExpression(@"^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z-0-9_\s]*$", ErrorMessage = "Use Latin, Arabic or Numeric Characters only ")]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        //[Phone(ErrorMessage = "Enter a valid phone number ")]


        [DataMember]
        public BindableCollection<string> PhoneNumbers
        {
            get => _phoneNumbers;
            set => Set(ref _phoneNumbers, value);
        }

        public string FirstNumber => PhoneNumbers?.FirstOrDefault();
        private decimal _debit;
        private string _mobile = "";
        private BindableCollection<string> _phoneNumbers;

        [DataMember]
        public decimal Debit
        {
            get => _debit;
            set { _debit = value;
                NotifyOfPropertyChange(nameof(Debit));
            }
        }


        [DataMember]
        //[Phone(ErrorMessage = "Enter a valid phone number ")]
        [Required(AllowEmptyStrings = false)]
        [MinLength(09, ErrorMessage = "Phone number length must be at least 9 digits ")]
        [MaxLength(12, ErrorMessage = "Phone number length must not exceed 12 digits ")]
        public string Mobile
        {
            get => _mobile;
            set
            {
                Set(ref _mobile, value);
            }
        }
    }    
}
