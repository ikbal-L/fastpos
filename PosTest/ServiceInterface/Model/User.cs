using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ServiceInterface.Model
{
    [DataContract]
    public class User : PropertyChangedBase
    {
        private string _backgroundString;
        private Brush _background;
        private Color? _backgroundColor;

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

