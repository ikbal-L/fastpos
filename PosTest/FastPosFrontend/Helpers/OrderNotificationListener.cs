using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    class OrderNotificationListener
    {
        private HttpClient _client;
        private string _url;
        private readonly Action<string> onUpdateReceived;


        public OrderNotificationListener(string url,Action<string> onUpdateReceived)
        {
            _url = url;
            this.onUpdateReceived = onUpdateReceived;
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromSeconds(60*60*24);

        }

        public async Task ListenForUpdates()
        {
            var url = "http://localhost:8080/api/events/subscribe";
            while (true)
            {
                try
                {
                    var stream = await _client.GetStreamAsync(url);
                    using (var streamReader = new StreamReader(stream))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            var message = await streamReader.ReadLineAsync();
                            onUpdateReceived?.Invoke(message);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        } 
        
    }
}
