using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using PasswordGenerator;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    public class UserDetailViewModel: PropertyChangedBase,INotifyDataErrorInfo
    {
        private string _username;
        private string _password;
        private string _pinCode;
        private string _firstName;
        private string _lastName;
        private string _email;
        private bool _isUserActive;
        private SolidColorBrush _background;

        private User _model;

        private readonly UserSettingsViewModel _parent;
        private Role _selectedRoleToAdd;
        private Role _selectedUserRole;
        private bool _isEditingPhone = false;
        private bool _isEditingRole = false;
        private string _newPhoneNumber;
        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        private bool _isSaveEnabled;

        public UserDetailViewModel(UserSettingsViewModel parent, User model = null )
        {
            _parent = parent;

            var data = StateManager.Get<Role>();
            Roles = new BindableCollection<Role>(data);

            InitializeModel(model);
        }

        private void InitializeModel(User model)
        {
            model??=new User(); 
            _model = model;
            CopyFromModel();
        }

        


        private void CopyFromModel()
        {
            Username = _model.Username;
            Password = _model.Password;
            FirstName = _model.FirstName;
            LastName = _model.LastName;
            Email = _model.Email;
            Background = new SolidColorBrush((Color) ColorConverter.ConvertFromString(_model.BackgroundString));
            IsUserActive = _model.Enabled;
            PinCode = _model.PinCode;
            if (_model.Roles != null)
            {
                UserRoles = new BindableCollection<Role>(_model.Roles);
            }

            if (_model.PhoneNumbers!= null)
            {
                PhoneNumbers = _model.PhoneNumbers;
            }
        }

        private void CopyToModel()
        {
            _model.Username = Username;
            _model.Password = Password ;
            _model.FirstName = FirstName;
            _model.LastName = LastName ;
            _model.Email = Email ;
            _model.PhoneNumbers = PhoneNumbers;
            _model.Roles = UserRoles.ToList();
            _model.RoleIds = UserRoles.Select(role => role.Id.Value).ToList();
            _model.Background = Background;
            _model.Enabled = IsUserActive;
            _model.PinCode = PinCode;
            //_model.BackgroundString = Background.Color.ToString();
        }
        [Required]
        [MinLength(3)]
        public string Username
        {
            get => _username;
            set
            {
                Set(ref _username, value);
                ValidateProperty(this, value);
            }
        }

        [Required]
        [MinLength(6)]
        public string Password
        {
            get => _password;
            set
            {
                Set(ref _password, value);
                if (_model?.Id == null)
                {
                    ValidateProperty(this, value);
                }
            }
        }

        public string PinCode
        {
            get => _pinCode;
            set => Set(ref _pinCode, value);
        }
        [Required]
        [MinLength(3)]
        public string FirstName
        {
            get => _firstName;
            set
            {
                Set(ref _firstName, value);
                ValidateProperty(this, value);
            }
        }

        [Required]
        [MinLength(3)]
        public string LastName
        {
            get => _lastName;
            set
            {
                Set(ref _lastName, value);
                ValidateProperty(this, value);
            }
        }

        [Required]
        [EmailAddress]
        public string Email
        {
            get => _email;
            set
            {
                Set(ref _email, value);
                ValidateProperty(this,value);
            }
        }

        public Role SelectedRoleToAdd
        {
            get => _selectedRoleToAdd;
            set => Set(ref _selectedRoleToAdd, value);
        }

        public string NewPhoneNumber
        {
            get => _newPhoneNumber;
            set => Set(ref _newPhoneNumber, value);
        }

        public Role SelectedUserRole
        {
            get => _selectedUserRole;
            set => Set(ref _selectedUserRole, value);
        }

        public BindableCollection<Role> Roles { get; set; }
        public BindableCollection<Role> UserRoles { get; set; } = new BindableCollection<Role>();
        public BindableCollection<string> PhoneNumbers { get; set; } = new BindableCollection<string>();

        public bool IsUserActive
        {
            get => _isUserActive;
            set => Set(ref _isUserActive, value);
        }

        public SolidColorBrush Background
        {
            get => _background;
            set => Set(ref _background, value);
        }


        public bool IsEditingPhone
        {
            get => _isEditingPhone;
            set => Set(ref _isEditingPhone, value);
        }

        public bool IsEditingRole
        {
            get => _isEditingRole;
            set => Set(ref _isEditingRole, value);
        }

        public void GeneratePassword()
        {
            var pwd = new Password()
                .IncludeLowercase()
                .IncludeUppercase()
                .IncludeNumeric()
                .IncludeSpecial()
                .LengthRequired(60);
            Password = pwd.Next();
        }

        public void AddRole()
        {
            if (SelectedRoleToAdd?.Id!= null)
            {
                if (!UserRoles.Contains(SelectedRoleToAdd))
                {
                    UserRoles.Add(SelectedRoleToAdd);
                }
                else
                {
                    ToastNotification.Notify("Selected Role is already assigned");
                }
                
            }
            else
            {
                ToastNotification.Notify("Select a valid role to add");
            }
            
            
            IsEditingRole = false;
        }

        public void RemoveRole()
        {
            if (SelectedUserRole?.Id!= null)
            {
                UserRoles.Remove(SelectedUserRole);
            }
        }

        public void AddPhone()
        {
            //TODO VALIDATION
            if (!PhoneNumbers.Contains(NewPhoneNumber))
            {
                PhoneNumbers.Add(NewPhoneNumber);
            }
            else
            {
                ToastNotification.Notify("Selected Phone is already assigned");
            }
            
            IsEditingPhone = false;
        }
        public void RemovePhone(string phone)
        {
            PhoneNumbers.Remove(phone);
        }

        public void SaveUser()
        {
            
            CopyToModel();
            if (StateManager.Save<User>(_model))
            {
                if (!_parent.Users.Contains(_model))
                {
                    _parent.Users.Add(_model);
                }
            }
            

            _parent.IsEditing = false;
            
        }

        public void Cancel()
        {
            _parent.IsEditing = false;
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }

        public bool HasErrors => _validationErrors.Any();

        public bool IsSaveEnabled
        {
            get => _isSaveEnabled;
            set => Set(ref _isSaveEnabled, value);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void ValidateProperty(object instance, object value,[CallerMemberName] string propertyName="")
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext =
                new ValidationContext(instance, null, null) { MemberName = propertyName };
            if (!Validator.TryValidateProperty(value, validationContext, validationResults))
            {
                _validationErrors.Add(propertyName, new List<string>());
                foreach (ValidationResult validationResult in validationResults)
                {
                    _validationErrors[propertyName].Add(validationResult.ErrorMessage);
                }
            }

            IsSaveEnabled = _validationErrors.Count == 0;
            NotifyOfPropertyChange(() => IsSaveEnabled);
            RaiseErrorsChanged(propertyName);
        }
    }
}