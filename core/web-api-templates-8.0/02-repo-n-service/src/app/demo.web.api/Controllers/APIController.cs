using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace demo.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        protected string GetErrorResultJsonString(Exception ex)
        {
            var jResult = new JObject();
            jResult.Add("isError", true);
            jResult.Add("errorCode", "SYSTEM_ERROR");
            jResult.Add("moreInfo", ex.Message);
            return jResult.ToString();
        }
    }
}
