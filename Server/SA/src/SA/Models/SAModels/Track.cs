using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Models.SAModels
{
    //Artists DB structure
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
}
