using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.Helpers
{
    public class ServiceHelper
    {
        public static void HandleStatusCodeErrors(int httpStatusCode, string message, params object[] args)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            if (httpStatusCode != 200)
            {
#if DEBUG
                throw new Exception($"{(HttpStatusCode)httpStatusCode}");
#endif

                logger.Info(message, args);

            }
        }
    }
}
