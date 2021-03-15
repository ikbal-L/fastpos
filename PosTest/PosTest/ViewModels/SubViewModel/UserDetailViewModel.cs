using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Caliburn.Micro;
using ServiceInterface.Model;
using ServiceLib.Service;

namespace PosTest.ViewModels.SubViewModel
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

        public UserDetailViewModel(User model)
        {
            _model = model;
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
                Roles = new BindableCollection<Role>(_model.Roles);
            }

            if (_model.PhoneNumbers!= null)
            {
                PhoneNumbers = new BindableCollection<string>(_model.PhoneNumbers);
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

        public BindableCollection<Role> Roles { get; set; } = new BindableCollection<Role>();
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

        public void GeneratePassword()
        {

        }

        public void SaveUser()
        {
            CopyToModel();
            StateManager.Save<User>(_model);
        }

        public void Cancel()
        {

        }
    }
}