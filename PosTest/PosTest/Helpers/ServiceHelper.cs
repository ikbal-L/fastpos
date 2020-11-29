using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;

namespace PosTest.Helpers
{
    public class ServiceHelper
    {
        public static void HandleStatusCodeErrors(int httpStatusCode, string message, params object[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            if (httpStatusCode != 200&& httpStatusCode!=0)
            {
#if DEBUG
                if (httpStatusCode ==422)
                {
                    throw new Exception($"{(HttpStatusCode)httpStatusCode} Errors: {args.Last()}");
                }

                throw new Exception($"{(HttpStatusCode)httpStatusCode}");


#endif

                logger.Info(message, args);

            }
        }

        public static void HandleValidationErrors(IEnumerable<string> errors)
        {

        }
    }
}
