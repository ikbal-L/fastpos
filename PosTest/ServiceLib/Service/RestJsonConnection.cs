using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ServiceLib.Service
{
    public class RestJsonConnection<T>
    {
        HttpClient client;
        string path;
        string uri;

        public RestJsonConnection(string uri, string path)
        {
            this.uri = uri;
            this.path = path;
            client = new HttpClient();
            client.BaseAddress = new Uri(uri);
        }

        public async Task<T> GetValuesAsync()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            T products = default(T);
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                products = await response.Content.ReadAsAsync<T>();
            }

            return products;
        }

    }
}
