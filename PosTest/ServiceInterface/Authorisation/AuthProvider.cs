using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Authorisation
{
    public abstract class AuthProvider
    {
        private static AuthProvider _instance;

        /// <summary>
        /// This method determines whether the user is authorize to perform the requested operation
        /// </summary>
        public abstract bool CheckpPermissions(UIElementPermission uipermission);

        public abstract string AuthorizationToken { get; }

        public static void Initialize<TProvider>() where TProvider : AuthProvider, new()
        {
            _instance = new TProvider();
        }

        public static void Initialize<TProvider>(object parameter)
        {
            _instance = (AuthProvider)typeof(TProvider).GetConstructor(new Type[] { typeof(User) }).Invoke(new object[] { parameter });
        }

        public static AuthProvider Instance
        {
            get { return _instance; }
        }
    }
}
