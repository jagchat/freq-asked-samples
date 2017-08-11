using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApiTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            using (var client=new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:47542/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var formContent = new FormUrlEncodedContent(new []
                {
                    new KeyValuePair<string, string>("x", "20"),
                    new KeyValuePair<string, string>("y", "70")
                });
                HttpResponseMessage response = await client.PostAsync("/api/calc/sum", formContent);
                var respJson = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(respJson);
                Assert.IsTrue(jObject?.GetValue("result").ToString() == "90");
            }
        }
    }
}
