using ServiceInterface.Model;
using System;

namespace ServiceInterface.Authorisation
{
    public class UIElementPermission
    {
        /// <summary>
        /// the list of permission descriptions related to UIElement. We should use the same nemes used in the database
        /// </summary>
        public string[] PermissionDescriptions { get; set; } //example {"AddOrder","EditOrder","ViewOrder"}

        /// <summary>
        /// If the visibility is false, which type of non visibility is chosen : Collapse or Hide. If it is related to IsEnamled or IsReadOnly, then select BOOL
        /// </summary>
        public VisibilityOrBool VisibilityOrBool { get; set; }

        /// <summary>
        ///  By default, the returned value (permitted/not permited) value will be checked by the intersection between UIElement permissions to user role permissions.
        ///  But if we have a case where current UI Element permission should be treated in another manner, in this case we define this function
        /// </summary>
        public Func<User, UIElementPermission, bool> IsPermitted { get; set; }

    }
}
