using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FastPosFrontend.Navigation
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PreAuthorizeAttribute:Attribute
    {
        public string[] Privileges { get; set; }

        public PreAuthorizeAttribute(params string[] privileges)
        {
            //var any = privileges.Where(p => Regex.IsMatch(p,@"(\w*\|\w*)+")).Select(p=>p.Split('|'));
            //var all = privileges.Where(p => Regex.IsMatch(p, @"(\w+,?)+")).Select(p => p.Split(','));
            Privileges = privileges;
        }

       
    }

    public interface IPrivilegeSet
    {
        public string[] Privileges { get;}

        public bool IsAuthorized(IPrincipal principal);
    }
    public interface IPrevilegeRequireAll:IPrivilegeSet
    {

    }
    public interface IPrevilegeRequireAny : IPrivilegeSet
    {

    }

    public class Authorize
    {
        internal class PrevilegeRequireAll : IPrevilegeRequireAll
        {
            private readonly string[] _privileges;
            public PrevilegeRequireAll(string[] privileges)
            {
                _privileges = privileges;
            }
            public string[] Privileges => _privileges;

            public bool IsAuthorized(IPrincipal principal)
            {
                return Privileges.All(p => principal.IsInRole(p));
            }
        }

        internal class PrevilegeRequireAny : IPrevilegeRequireAny
        {
            private readonly string[] _privileges;
            public PrevilegeRequireAny(string[] privileges)
            {
                _privileges = privileges;
            }
            public string[] Privileges => _privileges;

            public bool IsAuthorized(IPrincipal principal)
            {
                return Privileges.Any(p => principal.IsInRole(p));
            }
        }

        public static IPrevilegeRequireAll RequireAll(params string[] privileges)
        {
            return new PrevilegeRequireAll(privileges);
        }

        public static IPrevilegeRequireAny RequireAny(params string[] privileges)
        {
            return new PrevilegeRequireAny(privileges);
        }

        public static bool IsAuthorized(IEnumerable<IPrivilegeSet> privileges)
        {
            var principal = Thread.CurrentPrincipal;

            return privileges.All(p => p.IsAuthorized(principal));
        }
    }

}
