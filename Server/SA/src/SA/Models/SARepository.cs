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

        public void CheckDBConnection()
        {
            Database.CheckDBConnection();
        }
        public string GetTrackID(string username, double bpm, int action)
        {
            string LasttrackID = "";
            if (action != 3)
            {
                LasttrackID = GetUserTrack(username);
                if (action == 2) UpdateTrackInfo(LasttrackID, 2);
            }
            var collection = Database.database.GetCollection<BsonDocument>("spotify_db");
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Gt("tempo", bpm - 0.5) & filterBuilder.Lte("tempo", bpm + 0.5);
            var statelist = collection.Find(filter).ToList();
            double delta = 0.6;
            string result = LasttrackID;
            while (result == LasttrackID)
            {
                while (statelist.Count() == 0)
                {
                    filter = filterBuilder.Gt("tempo", bpm - delta) & filterBuilder.Lte("tempo", bpm + delta);
                    statelist = collection.Find(filter).ToList();
                    delta += 0.1;
                }
                Random rand = new Random();
                while (statelist.Count() > 0 && result == LasttrackID)
                {
                    var ResultItem = statelist[rand.Next(0, statelist.Count())];
                    result = BsonSerializer
                        .Deserialize<Track>(ResultItem)
                        .trackid;
                    statelist.Remove(ResultItem);
                }
            }
            UpdateTrackInfo(result, 0);
            SetUserTrack(username, result);
            return result;
        }
        public bool CheckUser(string username)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
            var collection = Database.database.GetCollection<BsonDocument>("users_db");

            var state = collection.Find(filter)
                                    .Limit(1)
                                    .Any();
            return state;
        }
        public void UpdateUserBPM(string username, double bpm)
        {
            var collection = Database.database.GetCollection<User>("users_db");
            var filter = Builders<User>.Filter.Eq(s => s.Username, username);
            var update = Builders<User>.Update.Push(s => s.trainings[0].bpms, bpm);
            collection.UpdateOneAsync(filter, update).Wait();
        }
        public string GetUserTrack(string username)
        {
            var fields=Builders<User>.Projection.Slice(s=>s.trainings, 1);
            var filter = Builders<User>.Filter.Eq(s => s.Username, username);
            var collection = Database.database.GetCollection<User>("users_db");

            var state =  BsonSerializer
                        .Deserialize<User>( 
                          collection.Find(filter)
                          .Project(fields)
                          .Limit(1)
                          .SingleAsync()
                          .Result);
            return state.trainings[0].tracks.Last();
        }
        public void SetUserTrack(string username, string trackid)
        {
            var filter = Builders<User>.Filter.Eq(s => s.Username, username);
            var update = Builders<User>.Update.Push(s => s.trainings[0].tracks, trackid);
            var collection = Database.database.GetCollection<User>("users_db");
            collection.UpdateOneAsync(filter, update).Wait();
        }

        public void UpdateTrackInfo(string trackid, int fl)
        {
            var filter = Builders<Track>.Filter.Eq(s => s.trackid, trackid);
            UpdateDefinition<Track> update;
            if (fl == 2)
            {
                update = Builders<Track>.Update.Inc(s => s.skiped, 1);
            }
            else
            {
                update = Builders<Track>.Update.Inc(s => s.played, 1);
            }
            var collection = Database.database.GetCollection<Track>("spotify_db");
            collection.UpdateOneAsync(filter, update).Wait();
        }
        public void StartTraining(string username)
        {
            var collection = Database.database.GetCollection<User>("users_db");
            var filter = Builders<User>.Filter.Eq(s => s.Username, username);
            var state = collection.Find(filter).FirstOrDefaultAsync().Result;
            Training newtrainig = new Training()
            {
                start = DateTime.Now,
                bpms = new List<double>(),
                tracks = new List<string>()
            };
            var update = Builders<User>.Update.PushEach(s => s.trainings, 
                                                        new List<Training>() { newtrainig },
                                                        position: 0);
            if (state != null)
            {
                collection.UpdateOneAsync(filter, update).Wait();
            }
            else
            {
                User newUser = new User() { Username = username, trainings = new List<Training>() { newtrainig } };
                collection.InsertOneAsync(newUser).Wait();
            }
        }
        public void EndTraining(string username)
        {
            var collection = Database.database.GetCollection<User>("users_db");
            var filter = Builders<User>.Filter.Eq(s => s.Username, username);
            var update = Builders<User>.Update.Set(s => s.trainings[0].end, DateTime.Now);
            collection.UpdateOneAsync(filter, update).Wait();
        }
    }
}
