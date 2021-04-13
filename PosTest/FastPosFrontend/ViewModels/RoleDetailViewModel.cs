using System.Linq;
using Caliburn.Micro;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    public class RoleDetailViewModel: PropertyChangedBase
    {
        private readonly RoleSettingsViewModel _parent;
        private string _name;
        private Role _model;

        public RoleDetailViewModel(RoleSettingsViewModel parent)
        {
            _parent = parent;
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public BindableCollection<Permission> Permissions { get; set; } 
        public BindableCollection<Permission> RolePermissions { get; set; } 

        public void Edit(Role role)
        {
            Name = role?.Name;
            Permissions = role?.Permissions == null
                ? new BindableCollection<Permission>()
                : new BindableCollection<Permission>(role.Permissions);
            _model = role;
        }

        public void Create()
        {
            Permissions = new BindableCollection<Permission>();
        }

        public void SaveRole()
        {
            CopyToModel();
            if (StateManager.Save(_model))
            {
                if (!_parent.Roles.Contains(_model))
                {
                    _parent.Roles.Add(_model);
                }
            }
            _parent.IsEditing = false;
        }

        public void Cancel()
        {
            _parent.IsEditing = false;
        }

        private void CopyToModel()
        {
            _model.Name = Name;
            _model.Permissions = Permissions.ToList();
        }
    }
}