using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;

namespace FastPosFrontend.Helpers
{
    public static class DeviceUtilities
    {
        //public static bool HasTouchCapabilities => PointerDevice.GetPointerDevices()
        //      .Any(p => p.PointerDeviceType == PointerDeviceType.Touch);
        public static bool HasTouchCapabilities => false;
    }
}
