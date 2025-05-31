using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.logging
{
    public class HttpRequestContextEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpRequestContextEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            // Get HttpContext properties here
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                // Add properties to the log event based on HttpContext
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestMethod", httpContext.Request.Method));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", httpContext.Request.Path));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserAgent", httpContext.Request.Headers["User-Agent"]));

                //Let us say we get correlationid passed in via request header, let us see how we can pull and populate that
                if (httpContext.Request.Headers.TryGetValue("x-correlation-id", out var appCorrelationId))
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationIdReceived", appCorrelationId));
                }
            }

        }
    }
}
