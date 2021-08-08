using ServiceInterface.Model;
using System.Linq;

namespace ServiceInterface.Authorisation
{
    public class DefaultAuthProvider : AuthProvider
    {
        private static User _user;
        private static string[] _userPermissions;
        private string _token;
        private long _annexId;

        /// <summary>
        /// Load the operation Access Control List (ACL)
        /// </summary>
        public DefaultAuthProvider(User user, string token, long annexId)
        {
            _token = token;
            _annexId = annexId;
            _user = user;
            if (_user != null && _user.Roles != null)
            {
                foreach (Role role in _user?.Roles)
                    foreach (Privilege permissioncategory in role?.Privileges)
                    {
                        if (_userPermissions == null)
                            if (permissioncategory.Permissions != null)
                                _userPermissions = new string[0];
                            else
                                continue;

                        _userPermissions = _userPermissions.Concat(permissioncategory.Permissions).ToArray();
                    }
            }
        }

        public override string AuthorizationToken => _token;

        public override long AnnexId => _annexId;

        /// <summary>
        /// This method determines whether the user is authorize to perform the requested operation
        /// </summary>
        public override bool CheckpPermissions(UIElementPermission uipermission)
        {
            if (uipermission == null || uipermission.PermissionDescriptions.Length == 0)
                return false;

            if (uipermission.IsPermitted != null)
                return uipermission.IsPermitted(_user, uipermission);

            if (_userPermissions != null && _userPermissions.Length > 0)
            {
                //  Match one of the requested UI-permissions with one of user permissions
                // Otherwise: if intersection between uipermissions and _userPermissions is not empty it returns true
                return _userPermissions.Any(userperm => uipermission.PermissionDescriptions.Any(uiperm => uiperm.ToUpperInvariant() == userperm.ToUpperInvariant()));
            }
            return false;
        }

    }
}
