﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using NLog;

namespace PosTest.Helpers
{
    public class ServiceHelper
    {
        public static void HandleStatusCodeErrors(int status, string message, params object[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            if (status != 200&& status!=0)
            {
                HandleErrorInDevelopment(status,Array.Empty<string>());

                logger.Info(message, args);

            }
        }

        [Conditional("DEBUG")]
        private static void HandleErrorInDevelopment(int status,IEnumerable<string> errors )
        {
            switch (status)
            {
                case 200: 
                case 2001: break;
                case 422: 
                    throw new ValidationException();
                default: throw new Exception($"{(HttpStatusCode) status}");
            }
        }
    }
}
