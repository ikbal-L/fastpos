using ServiceInterface.Model;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    class SyncManager
    {
        private static readonly Dictionary<object, bool> Locks;

        private static readonly RestApis api = new RestApis();

        static SyncManager()
        {
            Locks = new Dictionary<object, bool>();
        }

        public static bool Lock<TIdentifier>(IState<TIdentifier> obj) where TIdentifier : struct
        {
            if (!Locks.ContainsKey(obj))
            {
                Locks.Add(obj, false);
            }

            var id = obj.Id;
            var payload = !Locks[obj];
            Locks[obj] = payload;
            var url = api.Resource<Order>(EndPoint.LOCK, id);
            var result = GenericRest.RestPut(payload, url );
            return result.StatusCode == System.Net.HttpStatusCode.OK;

        }
    }
}
