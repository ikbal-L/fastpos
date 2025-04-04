﻿using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.ViewModels.Forms;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;
using Utilities.Extensions;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem("Role Settings", typeof(RoleSettingsViewModel),"",parentNavigationItem:typeof(UserSettingsViewModel), isQuickNavigationEnabled: true)]
    [PreAuthorize("Create_User|Create_Role|Delete_Role")]
    public class RoleSettingsViewModel:LazyScreen
    {
        private BindableCollection<Role> _roles;
        private Role _selectedRole;
        private RoleFormViewModel _roleDetailViewModel;
        private bool _isEditing;

        public RoleSettingsViewModel() : base()
        {

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

        public RoleFormViewModel RoleDetailViewModel
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
            Roles = new BindableCollection<Role>(StateManager.GetAll<Role>());
        }

        public void CreateRole()
        {
            RoleDetailViewModel = new RoleFormViewModel(this);
            RoleDetailViewModel.Create();
            IsEditing = true;
        }

        public void EditRole()
        {
            if (SelectedRole!= null)
            {
                if (SelectedRole.IsPredefined)
                {
                    ToastNotification.Notify("Can not Edit Predifined Role");

                    return;
                }

                RoleDetailViewModel = new RoleFormViewModel(this);
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
            if (SelectedRole == null)
            {
                ToastNotification.Notify("Select a Role to Delete First!");

                return;
            }

            if (SelectedRole.IsPredefined)
            {
                ToastNotification.Notify("Can not Delete Predifined Role");

                return;
            }

            var main = this.Parent as MainViewModel;
            main?.OpenDialog(
                DefaultDialog
                    .New("Are you sure you want perform this action?")
                    .Title("Delete Role")
                    .Ok(o =>
                    {
                        StateManager.Delete(SelectedRole).IfTrue(() => Roles.Remove(SelectedRole));
                        main.CloseDialog();
                    })
                    .Cancel(o =>
                    {
                        main.CloseDialog();
                    }));



            //var response = ModalDialogBox.YesNo("Are you sure you want to Perform this Action?", "Delete Order").Show();
            //if (response)
            //{
            //    StateManager.Delete(SelectedRole).IfTrue(() => Roles.Remove(SelectedRole));
            //}
        }
    }
}