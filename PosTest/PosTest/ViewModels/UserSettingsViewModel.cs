using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using PosTest.ViewModels.Settings;
using PosTest.ViewModels.SubViewModel;
using PosTest.Views;
using ServiceInterface.Model;
using ServiceLib.Service;

namespace PosTest.ViewModels
{
    public class UserSettingsViewModel : SettingsItemBase
    {
        private User _selectedUser;
        private UserDetailViewModel _userDetailViewModel;

        public UserSettingsViewModel()
        {
            this.Title = "User Settings";
            this.Content = new UserSettingsView(){DataContext = this};
            var userRepository = new UserRepository();
            StateManager.Instance.Manage(userRepository);
            //var data = StateManager.Get<User>();
            
            Users = new BindableCollection<User>(fakeData());
            UserDetailViewModel = new UserDetailViewModel(SelectedUser);

        }
        public BindableCollection<User> Users { get; set; }

        public User SelectedUser
        {
            get => _selectedUser;
            set => Set(ref _selectedUser, value);
        }

        public UserDetailViewModel UserDetailViewModel
        {
            get => _userDetailViewModel;
            set => Set(ref _userDetailViewModel, value);
        }

        public override void OnItemChanged()
        {
            StateManager.Flush<User>();
        }

        public IEnumerable<User> fakeData()
        {
            yield return new User() {Username = "User1", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com"};
            yield return new User() {Username = "User2", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com"};
            yield return new User() {Username = "User3", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com"};
            yield return new User() {Username = "User4", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com"};
            yield return new User() {Username = "User5", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com"};
            yield return new User() {Username = "User6", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com",PhoneNumbers = new ObservableCollection<string>(PhoneNumbers()),Roles = new List<Role>(Roles())};
            yield return new User() {Username = "User7", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com", PhoneNumbers = new ObservableCollection<string>(PhoneNumbers()) };
            yield return new User() {Username = "User8", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com", PhoneNumbers = new ObservableCollection<string>(PhoneNumbers()) };
            yield return new User() {Username = "User9", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com", PhoneNumbers = new ObservableCollection<string>(PhoneNumbers()) };
            yield return new User() {Username = "User10", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com", PhoneNumbers = new ObservableCollection<string>(PhoneNumbers()) };
            yield return new User() {Username = "User11", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com"};
            yield return new User() {Username = "User12", FirstName = "Ali", LastName = "Kamel", Email = "ali@email.com"};
        }

        public IEnumerable<string> PhoneNumbers()
        {
            yield return "0665666768";
            yield return "0666676869";
            yield return "0667686970";
            yield return "0667686971";
            yield return "0667686972";
        }

        public IEnumerable<Role> Roles()
        {
            yield return new Role(){Description = "Admin"};
            yield return new Role(){Description = "Accounting"};
            yield return new Role(){Description = "Management"};
        }

        public void CreateUser()
        {
            //UserDetailViewModel = new UserDetailViewModel(SelectedUser);
        }

        public void EditUser()
        {

        }

        public void DeleteUser()
        {

        }
    }
}