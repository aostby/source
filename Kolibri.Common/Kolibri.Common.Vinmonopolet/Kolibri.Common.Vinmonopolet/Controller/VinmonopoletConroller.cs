using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.IO;
using System.ComponentModel.Design;
using System.Net;
using System.Threading.Tasks;
using System.Drawing;

namespace Kolibri.Common.Vinmonopolet
{
    public class VinmonopoletConroller
    {
        private readonly string _subscriptionKey;//VinmonopoletConroller - \Documents\Innstillinger\VinmonopoletAPI
        private HttpClient _httpClient;
        private string _baseUrl = "https://apis.vinmonopolet.no";
        private string _productsApi = "products/v0";

        public VinmonopoletConroller(string SubscriptionKey)
        {
            _subscriptionKey = SubscriptionKey;
        }
        private async Task<HttpResponseMessage> Excecute(string method)
        {
            HttpClient client = GetClient();
            var uri = $"{_baseUrl}/{_productsApi}/{method}";
            var response = await client.GetAsync(uri);
            return response;
        }
        public async Task<string> DetailsNormal()
        {
            string method = "details-normal";
            var response = await Excecute(method);
            return await response.Content.ReadAsStringAsync();
        }


        private HttpClient GetClient()
        {
            if (_httpClient != null) { return _httpClient; }
            else
            {
                var client = new HttpClient();

                // Request headers
                client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

                return client;
            }

        }
     //   homepage = https://www.vinmonopolet.no/search?searchType=product&q=1566602

        public Image GetImage(string id) {
            return VinmonopoletImages.GetImage(id);
        }
    }

}
