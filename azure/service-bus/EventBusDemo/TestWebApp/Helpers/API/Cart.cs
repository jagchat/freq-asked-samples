using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TestWebApp.Helpers.Models;

namespace TestWebApp.Helpers.API
{
    public class Cart
    {
        public async Task<List<CartItemModel>> FetchCartData()
        {
            List<CartItemModel> result = null;
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true; //only for dev/poc
                };
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var response = await httpClient.GetAsync("https://localhost:6001/Cart");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var prodIds = JsonConvert.DeserializeObject<List<String>>(responseBody);
                    result = prodIds.Select(s => new CartItemModel() { ProductId = s }).ToList();
                }
            }
            return result;
        }
    }
}
