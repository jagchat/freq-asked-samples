using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ASPNETCoreConfigDemo.MyConfigJson
{
    public class MyConfigJsonProvider
    {
        public string Key { get; set; }
        public MyConfigJsonProvider(MyConfigJsonOptions options)
        {
            Key = options.Key;
        }

        public string GetConfigJsonContent()
        {
            var result = GetConfigContentAsync().Result;
            return result;
        }
        private async Task<string> GetConfigContentAsync()
        {
            var client = new HttpClient();
            string content = null;
            HttpResponseMessage response = await client.GetAsync("http://www.google.com"); //dummy web api for now
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            content = $@"{{ 
                ""My2ndLevelOption1"": ""{Key}-value1"",
                ""My2ndLevelOptionSet"": {{
                    ""SetOption1"": ""{Key}-set-value-1"",
                    ""SetOption2"": ""{Key}-set-value-2""
                }},
                ""MyCustomAsmName"":""DataLib2.dll"",
                ""MyCustomTypeName"":""DataLib2.DataProcess""
            }}";
            return content; //return JSON string
        }
    }
}
