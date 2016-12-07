using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SA.Models.SAModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Models
{
    public class SARepository : ISARepository
    {

        public void ConnectDatabase()
        {
            Database.ConnectDataBase();
        }
        public string GetTrackID( string username, double bpm)
        {
            var collection = Database.database.GetCollection<BsonDocument>("spotify_db");
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Gt("tempo", bpm - 5.0) & filterBuilder.Lte("tempo", bpm + 5.0);
            var statelist = collection.Find(filter).ToList();
            Random rand = new Random();
            var result = BsonSerializer.Deserialize<Track>(statelist[rand.Next(0, statelist.Count())])
                .trackid;
            UpdateTrackInfo(result, 0);
            SetUserTrack(username, result);
            return result;
        }
        public void UpdateUserBPM(string username, double bpm)
        {
            var collection = Database.database.GetCollection<User>("users");
            var filter = Builders<User>.Filter.Eq(s => s.Username, username);
            var state = collection.Find(filter).FirstOrDefaultAsync().Result;
            //var state = BsonSerializer.Deserialize<Repository.User>(collection.Find(filter).FirstOrDefault());
            if (state != null)
            {
                var update = Builders<User>.Update.Push(s => s.Lastbpm, bpm);
                collection.UpdateOneAsync(filter, update).Wait();
            }
            else
            {
                User newUser = new User() { Username = username, Lastbpm = new List<double> { bpm } };
                collection.InsertOneAsync(newUser).Wait();
            }
        }
        public string GetUserTrack(string username)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Username, username);
            var collection = Database.database.GetCollection<User>("Users");
            var state = collection.Find(filter).SingleAsync().Result;
            return state.trackid;
        }
        public void SetUserTrack(string username, string trackid)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Username, username);
            var update = Builders<User>.Update.Set(s => s.trackid, trackid);
            var collection = Database.database.GetCollection<User>("users");
            collection.UpdateOneAsync(filter, update).Wait();
        }

        public void UpdateTrackInfo(string trackid, int fl)
        {
            if (fl == 2)
            {
                var filter = Builders<Track>.Filter.Eq(s => s.trackid, trackid);
                var update = Builders<Track>.Update.Inc(s => s.skiped, 1);
                var collection = Database.database.GetCollection<Track>("spotify_db");
                collection.UpdateOneAsync(filter, update).Wait();
            }
            else
            {
                var filter = Builders<Track>.Filter.Eq(s => s.trackid, trackid);
                var update = Builders<Track>.Update.Inc(s => s.played, 1);
                var collection = Database.database.GetCollection<Track>("spotify_db");
                collection.UpdateOneAsync(filter, update).Wait();
            }

        }
    }
}
