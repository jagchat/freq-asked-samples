using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace DynamicEntitiesREST.Infrastructure
{
    public class CustomXmlFormatter : MediaTypeFormatter //TODO: not being used as of now
    {
        public CustomXmlFormatter()
        {
            SupportedMediaTypes.Add(
                new MediaTypeHeaderValue("application/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
        }

        public override bool CanReadType(Type type)
        {
            if (type == (Type)null)
                throw new ArgumentNullException("type");

            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task WriteToStreamAsync(Type type, object value,
            Stream writeStream, System.Net.Http.HttpContent content,
            System.Net.TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                //--TBD: not a great way...need to enhance this (doing object to json to xml)
                var json = JsonConvert.SerializeObject(value);
                var xml = JsonConvert
                    .DeserializeXmlNode("{\"root\":" + json + "}", "root");

                xml.Save(writeStream);
            });
        }
    }
}
