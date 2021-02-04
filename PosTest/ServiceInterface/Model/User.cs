using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PosTest.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    [DataContract]
    public class User : PropertyChangedBase,IState<long>
    {
        private string _backgroundString;
        private Brush _background;
        private Color? _backgroundColor;
        private Agent _agent;
        private bool? _isActive;

        [DataMember]
        public long? Id { get; set; }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }

        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [DataMember]
        public string PinCode { get; set; }

        private ObservableCollection<string> _PhoneNumbers;
        [DataMember]
        public ObservableCollection<string> PhoneNumbers
        {
            get { return _PhoneNumbers; }
            set
            {
                _PhoneNumbers = value;
                NotifyOfPropertyChange(nameof(PhoneNumbers));
            }
        }

        public List<Role> Roles { get; set; }

        [DataMember]
        public List<long> RoleIds { get; set; }

        [DataMember]
        public bool IsActive
        {
            get { return (bool) (_isActive==null?true:_isActive); }
            set => Set(ref _isActive, value);
        }


        [DataMember]
        [Required(AllowEmptyStrings = false)]
        public string BackgroundString
        {
            get => _backgroundString ?? "#f39c12";
            set
            {
                _backgroundString = value;
                NotifyOfPropertyChange(nameof(Background));
            }

        }

        public virtual Brush Background
        {
            get => _background ?? (_background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString)));
            set
            {
                _background = (SolidColorBrush)value;
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

    public enum Agent
    {
        Desktop,Mobile,Web
    }

}

