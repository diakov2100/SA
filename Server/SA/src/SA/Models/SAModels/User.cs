using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Models.SAModels
{
    public class User
    {
        [BsonElement("_id")]
        public ObjectId _id { get; set; }
        public string Username { get; set; }
        public List<Training> trainings { get; set; }
    }
}
