using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace SimpleServer
{
    public class Repository
    {
        public class Track
        {
            [BsonElement("_id")]
            public ObjectId _id { get; set; }
            public double duration { get; set; }
            public double tempo { get; set; }
            public string trackid { get; set; }
            public string name { get; set; }
            public int played { get; set; }
            public int skiped { get; set; }
            public Artists[] artists { get; set; }
        }
        public class Artists
        {
            public string artistid { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string uri { get; set; }
            public string href { get; set; }
        }
        public class User
        {
            [BsonElement("_id")]
            public ObjectId _id { get; set; }
            public string username { get; set; }
            public IEnumerable<double> lastbpm { get; set; }
            public string trackid { get; set; }
        }

    }
}
