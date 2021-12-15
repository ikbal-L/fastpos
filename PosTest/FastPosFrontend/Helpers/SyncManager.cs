using Newtonsoft.Json;
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
        private static readonly RestApi api = new RestApi();

        static SyncManager()
        {

        }
        public static bool Lock<T>(T obj) where T : ILockableState<long> => Lock<T, long>(obj);

        public static bool Lock<T,Id>(T obj) where T: ILockableState<Id> where Id : struct
        {
            var payload = !obj.IsLocked;
            var url = api.Resource<Order>(EndPoint.LOCK, obj.Id);
            return SetLock<T, Id>(obj, payload, url);
        }

        private static bool SetLock<T, Id>(T obj, bool payload, string url)
            where T : ILockableState<Id>
            where Id : struct
        {
            var res = GenericRest.RestPut(payload, url);
            if (res.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<T>(res.Content);
                obj.IsLocked = result.IsLocked;
                obj.LockedBy = result.LockedBy;
            }

            return res.IsSuccessful;
        }
    }
}
