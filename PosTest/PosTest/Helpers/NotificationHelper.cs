using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PosTest.Helpers
{
    public static class NotificationHelper
    {
        public static readonly String CHECK_SERVER_CONNECTION;
        public static readonly String ADDITIVE_DELETE_SUCCESS;
        public static readonly String SELECTED_SAME_ADDITIVE;
        public static readonly String SELECTE_ZONE_TO_COPY;
        public static readonly String SELECT_ADDITIVE_TO_COPY;
        public static readonly String SELECT_ADDITIVE_TO_DELETE;
        public static readonly String SELECT_ADDITIVE_TO_MOVE;
        public static readonly String SELECT_ADDITIVE_TO_EDIT;
        public static readonly String NEWPRICE_LESSTHAN_TOTAL;
        public static readonly String PAYEDAMOUNT_LESSTHAN_TOTAL;

        static NotificationHelper()
        {
            var fieldInfos = typeof(NotificationHelper).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);
            foreach (var fieldInfo in fieldInfos)
            {
                string value = Application.Current.FindResource(fieldInfo.Name) as string;
                fieldInfo.SetValue(null, value);
            }
        }
    }
}