using DemoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DemoBot.Services
{
    public class NluEntity {
        public string value { get; set; }
        public string entity { get; set; }
    }

    public class MyNluParser: IParser //TODO: just for demo
    {
        private static readonly HttpClient client = new HttpClient();
        private static string url = "https://mynlu-front-layer-production.herokuapp.com/parse?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjdXN0b21lcklkIjoiNWEzMWI5ZTc2Y2EwNjIwMDEyNTZlMzA4IiwiY29udGFpbmVySWQiOiI1YTMxYmI2NjZjYTA2MjAwMTI1NmUzMGEiLCJpYXQiOjE1MTMyMDg2Nzh9.SAkGN1cmX_Pz5-OXA9Aw0FzxkF5XmVLeDyispyKEbqA";

        public async Task<ParseResult> ParseText(string text)
        {
            var r = new ParseResult();

            var content = new StringContent($@"{{""q"": ""{text}""}}", System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject(responseString);

            var entities = new List<NluEntity>();
            foreach (var entity in responseObject.entities) {
                entities.Add(new NluEntity() {value = entity.value, entity = entity.entity});
            }

            r.TableName = entities.FirstOrDefault(o => o.entity.StartsWith("table_"))?.entity.Split('_')[1];
            r.Operation = entities.FirstOrDefault(o => o.entity.StartsWith("operation_"))?.entity.Split('_')[1];
            return r;
        }

    }
}