using System.Linq;
using Caliburn.Micro;
using Caliburn.Micro;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels.Forms
{
    public class RoleFormViewModel: PropertyChangedBase
    {
        private readonly RoleSettingsViewModel _parent;
        private string _name;
        private Role _model;

        public RoleFormViewModel(RoleSettingsViewModel parent)
        {
            _parent = parent;
            Privileges = new BindableCollection<Privilege>(StateManager.GetAll<Privilege>());
        }
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public Role Model { get=>_model; set=> Set(ref _model,value); }
        public BindableCollection<Privilege> Privileges { get; set; } 
        public BindableCollection<Privilege> RolePrivileges { get; set; } 

        public void Edit(Role role)
        {
            Name = role?.Name;
            
            RolePrivileges = role?.Privileges == null
                ? new BindableCollection<Privilege>()
                : new BindableCollection<Privilege>(role.Privileges);
            _model = role;
        }

        public void Create()
        {
            _model = new Role();
            RolePrivileges = new BindableCollection<Privilege>();
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
            _model.Privileges = RolePrivileges.ToList();
            _model.PrivilegeIds = _model.Privileges.Where(p=>p.Id!= null).Select(p => (long) p.Id).ToList();
        }

        public void PrivilegeAction(Privilege privilege,bool isChecked)
        {
            if (isChecked)
            {
                RolePrivileges.Remove(privilege);
            }
            else
            {
                RolePrivileges.Add(privilege);
            }
        }
    }
}