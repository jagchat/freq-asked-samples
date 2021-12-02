using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.MongoDb
{
    public class DynamicSerializer : SerializerBase<JObject>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JObject value)
        {
            var myBsonDoc = MongoDB.Bson.BsonDocument.Parse(value.ToString());
            BsonDocumentSerializer.Instance.Serialize(context, myBsonDoc);
        }

        public override JObject Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var myBsonDoc = BsonDocumentSerializer.Instance.Deserialize(context);
            return JObject.Parse(myBsonDoc.ToString());
        }
    }
}
