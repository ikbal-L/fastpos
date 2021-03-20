using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.Views;
using ServiceInterface.Model;
using ServiceLib.Service;

namespace FastPosFrontend.ViewModels
{
    public class UserSettingsViewModel : /*SettingsItemBase*/ Screen
    {
        private User _selectedUser;
        private UserDetailViewModel _userDetailViewModel;
        private bool _isEditing;
        //private UserDetailView _subView;

        public UserSettingsViewModel()
        {
            //this.Title = "User Settings";
            //this.Content = new UserSettingsView() { DataContext = this };
            var userRepository = new UserBaseRepository();
            var roleRepository = new RoleRepository();
            StateManager.Instance.Manage(userRepository).Manage(roleRepository);

            StateManager.Associate<Role,User>();
            var data = StateManager.Get<User>();

            Users = new BindableCollection<User>(data);
            IsEditing = false;


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

        


    }
}