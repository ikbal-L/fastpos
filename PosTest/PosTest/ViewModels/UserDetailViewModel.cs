using System.Drawing;
using System.Windows;
using Caliburn.Micro;
using PosTest.Helpers;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace PosTest.ViewModels
{
    public class UserDetailViewModel: PropertyChangedBase
    {
        private string _username;
        private string _password;
        private string _pinCode;
        private string _firstName;
        private string _lastName;
        private string _email;
        private bool _isActive;
        private Brush _background;

        private User _model;

        private readonly UserSettingsViewModel _parent;
        private Role _selectedRoleToAdd;
        private Role _selectedUserRole;
        private bool _isEditingPhone = false;
        private bool _isEditingRole = false;
        private string _newPhoneNumber;

        public UserDetailViewModel(User model, UserSettingsViewModel parent)
        {
            _parent = parent;

            var data = StateManager.Get<Role>();
            Roles = new BindableCollection<Role>(data);

            InitializeModel(model);
        }

        private void InitializeModel(User model)
        {
            if (model != null)
            {
                _model = model;
                CopyFromModel();
            }
            else
            {
                _model = new User();
            }
        }


        private void CopyFromModel()
        {
            Username = _model.Username;
            Password = _model.Password;
            FirstName = _model.FirstName;
            LastName = _model.LastName;
            Email = _model.Email;
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
        }
        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public string PinCode
        {
            get => _pinCode;
            set => Set(ref _pinCode, value);
        }

        public string FirstName
        {
            get => _firstName;
            set => Set(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => Set(ref _lastName, value);
        }

        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
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

        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }

        public Brush Background
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
    }
}