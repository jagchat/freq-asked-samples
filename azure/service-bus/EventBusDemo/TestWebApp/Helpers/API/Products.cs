using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using TestWebApp.Helpers.Models;

namespace TestWebApp.Helpers.API
{
    public class Products
    {
        public async Task<string> AddProductToCartAsync(AddProductToCartInputModel m)
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(m?.ProductId))
            {
                dynamic dynInput = new ExpandoObject();
                dynInput.ProductId = m.ProductId;

                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => {
                        return true; //only for dev/poc
                    };
                    using (var httpClient = new HttpClient(httpClientHandler))
                    {
                        var stringContent = new StringContent(JsonConvert.SerializeObject(dynInput), Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync("https://localhost:5001/Product/SelectProduct", stringContent);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        result = responseBody;
                    }
                }
            }

            return result;
        }
    }
}
