using Caliburn.Micro;
using PosTest.ViewModels.Settings;
using ServiceInterface.Model;
using ServiceLib.Service;

namespace PosTest.ViewModels
{
    public class UserSettingsViewModel: SettingsItemBase
    {
        public UserSettingsViewModel()
        {
            var userRepository = new UserRepository();
           StateManager.Register(append:true).Manage<User>().Using(userRepository).Commit();
           var data = StateManager.Get<User>();
           Users = new BindableCollection<User>(data);

        }
        public BindableCollection<User> Users { get; set; }

        public override void OnItemChanged()
        {
            StateManager.Flush<User>();
        }
    }
}