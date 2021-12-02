using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Demo.MongoDb
{
    //TODO: refactor as necessary
    public class AuditRepository
    {
        private MongoClient client = null;

        public AuditRepository()
        {
            client = new MongoClient("mongodb://root:admin123@localhost:27017");  //TODO: configurable
        }

        public void AddBsonEntry(string json)
        {
            var db = client.GetDatabase("AuditTrail"); //TODO: configurable
            var docCollection = db.GetCollection<dynamic>("AuditEntries"); //TODO: configurable
            var doc = BsonSerializer.Deserialize<BsonDocument>(json);
            docCollection.InsertOneAsync(doc).GetAwaiter().GetResult();
        }

        public List<dynamic> FindEntries(string filterJson) //TODO: make this async
        {
            var db = client.GetDatabase("AuditTrail"); //TODO: configurable
            var docCollection = db.GetCollection<BsonDocument>("AuditEntries"); //TODO: configurable
            BsonDocument filter
                = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(filterJson);

            var sort = Builders<BsonDocument>.Sort
                .Descending(o => o["_v.UpdateDate"]);
            var docs = docCollection.FindAsync(filter,
                        new FindOptions<BsonDocument, BsonDocument> { Sort = sort })
                .GetAwaiter()
                .GetResult()
                .ToEnumerable<dynamic>();

            return docs.ToList();
        }

        public string FindEntriesJson(string filterJson) //TODO: make this async
        {
            var db = client.GetDatabase("AuditTrail"); //TODO: configurable
            var docCollection = db.GetCollection<BsonDocument>("AuditEntries"); //TODO: configurable
            BsonDocument filter
                = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(filterJson);

            var sort = Builders<BsonDocument>.Sort
                .Descending(o => o["_v.UpdateDate"]);
            var docs = docCollection.Find(filter)
                .Project(Builders<BsonDocument>.Projection.Exclude("_id"))
                .Sort(sort)
                .ToListAsync()
                .GetAwaiter()
                .GetResult();

            return docs.ToJson(); //TODO: better way?
        }

        //public string FindEntriesJson(string tableName, string idCol, string idValue) //TODO: make this async
        //{
        //    var db = client.GetDatabase("AuditTrail"); //TODO: configurable
        //    var docCollection = db.GetCollection<BsonDocument>("AuditEntries"); //TODO: configurable
        //    var filter = Builders<BsonDocument>.Filter.Eq("_v.TableName", tableName);
        //    filter &= Builders<BsonDocument>.Filter.Eq("_v.IdCol", idCol);
        //    filter &= Builders<BsonDocument>.Filter.Eq("_v.IdValue", idValue);

        //    var sort = Builders<BsonDocument>.Sort
        //        .Descending(o => o["_v.UpdateDate"]);
        //    var docs = docCollection.Find(filter)
        //        .Project(Builders<BsonDocument>.Projection.Exclude("_id"))
        //        .Sort(sort)
        //        .ToListAsync()
        //        .GetAwaiter()
        //        .GetResult();

        //    return docs.ToJson(); //TODO: better way?
        //}

        public string FindEntriesJson(int currentPage = 1, int pageSize = 5) //TODO: make this async
        {
            var db = client.GetDatabase("AuditTrail"); //TODO: configurable
            var docCollection = db.GetCollection<BsonDocument>("AuditEntries"); //TODO: configurable

            //TODO: improve this
            //double totalDocuments = docCollection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty).GetAwaiter().GetResult();
            //var totalPages = Math.Ceiling(totalDocuments / pageSize);

            var sort = Builders<BsonDocument>.Sort
                .Ascending(o => o["_v.TableName"])
                .Ascending(o => o["_v.IdCol"])
                .Ascending(o => o["_v.IdValue"])
                .Descending(o => o["_v.UpdateDate"]);
            var filter = Builders<BsonDocument>.Filter.Empty;
            var docs = docCollection.Find(filter)
                .Project(Builders<BsonDocument>.Projection.Exclude("_id"))
                .Sort(sort)
                .Skip((currentPage - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync()
                .GetAwaiter()
                .GetResult();

            return docs.ToJson(); //TODO: better way?
        }

    }
}
