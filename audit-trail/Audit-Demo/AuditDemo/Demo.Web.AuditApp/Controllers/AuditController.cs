using Demo.MongoDb;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Core;
using Microsoft.Extensions.Logging;

namespace Demo.Web.AuditApp.Controllers
{
    public class AuditController : Controller
    {
        private readonly ILogger<AuditController> _logger;
        private AuditRepository auditRepository = new AuditRepository(); //TODO: use DI

        public AuditController(ILogger<AuditController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogTrace("AuditController.Index: Started..");
            var result = auditRepository.FindEntriesJson();
            return View(JsonExtensions.ToJArray(result));
        }

        [Route("~/Audit/{tenantName}/{appName}/{key}/{value}")]
        public IActionResult Details(string tenantName, string appName, string key, string value)
        {
            _logger.LogTrace("AuditController.Details: Started..");
            var s = $@"
                {{
	                ""_v.TenantInfo.Name"" : ""{tenantName}"",
                    ""_v.AppInfo.Name"": ""{appName}"",
                    ""_v.Key"": ""{key}"",
                    ""_v.Value"": ""{value}""
                }}
            ";
            var result = auditRepository.FindEntriesJson(s);
            return View(JsonExtensions.ToJArray(result));
        }
    }
}
