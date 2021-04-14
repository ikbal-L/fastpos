using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Windows.Media;
using Newtonsoft.Json;

namespace ServiceInterface.Model
{
    [DataContract]
    public class User : PropertyChangedBase,IState<long>
    {
        private string _backgroundString = "#4a4c4f";
        private Brush _background;
        private Color? _backgroundColor;
        private Agent _agent;
        private bool? _isActive = false;

        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string FirstName
        {
            get => _firstName;
            set => Set(ref _firstName, value);
        }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string LastName
        {
            get => _lastName;
            set => Set(ref _lastName, value);
        }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [DataMember]
        public string PinCode { get; set; }

        
        private string _username;
        private BindableCollection<string> _phoneNumbers;
        private bool _enabled = true;
        private string _firstName;
        private string _lastName;

        [DataMember]
        public BindableCollection<string> PhoneNumbers
        {
            get => _phoneNumbers;
            set => Set(ref _phoneNumbers, value);
        }

        public List<Role> Roles { get; set; } = new List<Role>();

        [DataMember]
        public List<long> RoleIds { get; set; }

        [DataMember]
        public bool IsUserActive
        {
            get { return (bool) (_isActive==null?true:_isActive); }
            set => Set(ref _isActive, value);
        }


        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string BackgroundString
        {
            get => _backgroundString ?? /*"#f39c12";*/"#4a4c4f";
            set
            {
                _backgroundString = value;
                NotifyOfPropertyChange(nameof(Background));
            }

        }

        public virtual Brush Background
        {
            get => _background ??= new SolidColorBrush((Color)ColorConverter.ConvertFromString(_backgroundString));
            set
            {
               
                Set(ref _background, (SolidColorBrush) value);
                _backgroundString = ((SolidColorBrush) value)?.Color.ToString();
                NotifyOfPropertyChange(nameof(Background));
            }
        }

        public virtual Color? BackgroundColor
        {

            get
            {
                if (_backgroundColor == null)
                {
                    _backgroundColor = (Color)ColorConverter.ConvertFromString(BackgroundString);
                }
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
                BackgroundString = _backgroundColor.ToString();
                Background = new SolidColorBrush((Color)_backgroundColor);
                NotifyOfPropertyChange(() => IsDark);
            }
        }

        public bool IsDark
        {
            get
            {
                var c = BackgroundColor.GetValueOrDefault();
                var d = (5 * c.G + 2 * c.R + c.B) <= 8 * 128;
                return (5 * c.G + 2 * c.R + c.B) <= 8 * 140;
            }
        }

        
        public Agent Agent
        {
            get => _agent;
            set => Set(ref _agent, value);
        }

        [DataMember]
        public bool Enabled
        {
            get => _enabled;
            set => Set(ref _enabled, value);
        }
    }

   

    [DataContract]
    public class Waiter : User
    {
 
    }

    [DataContract]
    public class Deliveryman : User
    {
  
        private decimal _balance;
        [DataMember]

        public decimal Balance
        {
            get { return _balance; }
            set {
                _balance = value;
                NotifyOfPropertyChange(nameof(Balance));
            }
        }
    

    }

    [DataContract]
    public class Role:IState<long>
    {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<Privilege> Privileges { get; set; }

        [DataMember]
        [JsonProperty("PrivilegeIds")]
        public List<long> PrivilegeIds { get; set; }
    }

    [DataContract]
    public class Privilege : IState<long>
    {
        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string[] Permissions { get; set; }

    }

    public enum Agent
    {
        Desktop,Mobile,Web
    }

}

