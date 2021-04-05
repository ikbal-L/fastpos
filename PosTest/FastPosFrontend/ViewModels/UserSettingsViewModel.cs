using System.Collections.Generic;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.Views;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItemConfiguration("User Settings", typeof(UserSettingsViewModel), groupName: "Settings")]
    public class UserSettingsViewModel : /*SettingsItemBase*/ LazyScreen
    {
        private User _selectedUser = null;
        private UserDetailViewModel _userDetailViewModel = null;
        private bool _isEditing = false;
        //private UserDetailView _subView;

        public UserSettingsViewModel()
        {
            //this.Title = "User Settings";
            //this.Content = new UserSettingsView() { DataContext = this };
            var userRepository = new UserRepository();
            var roleRepository = new RoleRepository();
            StateManager.Instance.Manage(userRepository,withAssociatedTypes:true).Manage(roleRepository);

            
            IsEditing = false;
            Setup();


        }
        public BindableCollection<User> Users { get; set; }

        public User SelectedUser
        {
            get => _selectedUser;
            set => Set(ref _selectedUser, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                Set(ref _isEditing, value);
                if (!value)
                {
                    UserDetailViewModel = null;
                }
            }
        }

        public UserDetailViewModel UserDetailViewModel
        {
            get => _userDetailViewModel;
            set => Set(ref _userDetailViewModel, value);
        }

        //public override void OnItemChanged()
        //{
        //    StateManager.Flush<User>();
        //}

       

       

        public void CreateUser()
        {

            IsEditing = true;
            UserDetailViewModel = new UserDetailViewModel(null,this);
            //SubView = new UserDetailView(){DataContext = UserDetailViewModel};
        }

        public void EditUser()
        {
            if (SelectedUser?.Id== null)
            {
                ToastNotification.Notify("Select a user to edit");
                return;
            }
            UserDetailViewModel = new UserDetailViewModel(SelectedUser, this);
            //SubView = new UserDetailView() {DataContext = UserDetailViewModel};
            IsEditing = true;
            
        }

        //public UserDetailView SubView
        //{
        //    get => _subView;
        //    set => Set(ref _subView, value);
        //}

        public void DeleteUser()
        {
            if (SelectedUser?.Id!= null)
            {
                //StateManager.Delete<User>(SelectedUser);
                Users.Remove(SelectedUser);
                return;
            }
            ToastNotification.Notify("Select a user to delete");
        }


        protected override void Setup()
        {
            var users = StateManager.GetAsync<User>();
            _data = new NotifyAllTasksCompletion(users);
            if (_data.IsCompleted)
            {
                Initialize();

            }
            //_data.PropertyChanged += _data_PropertyChanged;
            _data.AllTasksCompleted += OnAllTasksCompleted;
        }

        public override void Initialize()
        {
            //StateManager.Associate<Role, User>();

            var data = _data.GetResult<ICollection<User>>();
            Users = new BindableCollection<User>(data);
        }
    }
}