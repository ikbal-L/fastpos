using Caliburn.Micro;
using FastPosFrontend.Helpers;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItemConfiguration("Role Settings", typeof(RoleSettingsViewModel),parentNavigationItem:typeof(UserSettingsViewModel))]
    public class RoleSettingsViewModel:LazyScreen
    {
        private BindableCollection<Role> _roles;
        private Role _selectedRole;
        private RoleDetailViewModel _roleDetailViewModel;
        private bool _isEditing;

        public RoleSettingsViewModel()
        {
            Setup();
            OnReady();
        }

        public BindableCollection<Role> Roles
        {
            get => _roles;
            set => Set(ref _roles, value);
        }

        public Role SelectedRole
        {
            get => _selectedRole;
            set => Set(ref _selectedRole, value);
        }

        public RoleDetailViewModel RoleDetailViewModel
        {
            get => _roleDetailViewModel;
            set => Set(ref _roleDetailViewModel, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => Set(ref _isEditing, value);
        }

        protected override void Setup()
        {
            var roles= StateManager.GetAsync<Role>();
            _data = new NotifyAllTasksCompletion(roles);
        }

        public override void Initialize()
        {
            Roles = new BindableCollection<Role>(StateManager.Get<Role>());
        }

        public void CreateRole()
        {
            RoleDetailViewModel = new RoleDetailViewModel(this);
            RoleDetailViewModel.Create();
            IsEditing = true;
        }

        public void EditRole()
        {
            if (SelectedRole!= null)
            {
                RoleDetailViewModel = new RoleDetailViewModel(this);
                RoleDetailViewModel.Edit(SelectedRole);
                IsEditing = true;
            }
            else
            {
                ToastNotification.Notify("Select a Role to Edit First!");
            }
        }

        public void DeleteRole()
        {   

        }
    }
}