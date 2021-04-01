using System;
using System.Net.Sockets;

namespace FastPosFrontend.Helpers
{
    public class ConnectionHelper
    {
        public static bool PingHost(string hostUri="localhost", int portNumber=8080)
        {
            try
            {
                using var client = new TcpClient(hostUri, portNumber);
                return true;
            }
            catch (SocketException ex)
            {
                //MessageBox.Show("Error pinging host:'" + hostUri + ":" + portNumber.ToString() + "'");
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}