using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyWebAPI;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using SimpleServer.models;
using System.Net;
using System.Xml.Linq;
using System.IO;
using MongoDB.Bson.Serialization;

namespace SimpleServer
{
    static class DataBase
    {
        static MongoClient client;
        static IMongoDatabase database;

        static string connectionString = "mongodb://max:123456@46.101.136.126/HackJunction_db";

        public static void InitDataBase()
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("HackJunction_db");
        }

        static public string DBrequest(double bpm, int action, string username, int style)
        {
            switch (action)
            {
                case 0:
                    UserBPM(username, bpm);
                    return "";
                    
                case 1:
                    UserBPM(username, bpm);
                    return GetTrackID(bpm, username);
                    break;
                case 2:
                    UserBPM(username, bpm);
                    UpdateTrackInfo(username, 2);
                    return GetTrackID(bpm,username);
                    
            }
            return "";
        }
        static public string GetTrackID(double bpm, string username)
        {
            var collection = database.GetCollection<BsonDocument>("spotify_db");
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Gt("tempo", bpm - 5.0) & filterBuilder.Lte("tempo", bpm + 5.0);
            var cursor = collection.Find(filter).ToCursor();
            var resultarray = cursor.ToEnumerable();
            List<Repository.Track> list = new List<Repository.Track>();
            foreach (var document in cursor.ToEnumerable())
            {
                list.Add(BsonSerializer.Deserialize<Repository.Track>(document));
            }
            Random rand = new Random();
            var result = list[rand.Next(0, list.Count())].trackid;
            UpdateTrackInfo(result, 0);
            SetUserTrack(username, result);
            return result;
        }
        static public void UserBPM(string username, double bpm)
        {
            var collection = database.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
            var state = BsonSerializer.Deserialize<Repository.User>(collection.Find(filter).First());
            if (state != null)
            {
                state.lastbpm.ToList().Add(bpm);
                var update = Builders<BsonDocument>.Update.Set("lastbpm", state.lastbpm as IEnumerable<double>);
                var result = collection.UpdateOneAsync(filter, update).Result;
            }    
            else
            {
                Repository.User newUser = new Repository.User() { username = username, lastbpm =new List<double> { bpm } }; 
                collection.InsertOneAsync(newUser.ToBsonDocument()).Wait();
            }        
        }
        static public string GetUserTrack(string username)
        {
            var collection = database.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
            var state = BsonSerializer.Deserialize<Repository.User>(collection.Find(filter).First());
            return state.trackid;
        }
        public static void SetUserTrack(string username, string trackid)
        {
            var collection = database.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
            var state = BsonSerializer.Deserialize<Repository.User>(collection.Find(filter).First());
            var update = Builders<BsonDocument>.Update.Set("trackid", trackid);
            var result = collection.UpdateOneAsync(filter, update).Result;

        }
        static public void UpdateTrackInfo(string trackid, int fl)
        {            
            if (fl == 2)
            {
                var collection = database.GetCollection<BsonDocument>("spotify_db"); ;
                var filter = Builders<BsonDocument>.Filter.Eq("trackid", trackid);
                var state = BsonSerializer.Deserialize<Repository.Track>(collection.Find(filter).First());
                var update = Builders<BsonDocument>.Update.Set("skiped", state.skiped++);
                var result = collection.UpdateOneAsync(filter, update).Result;
            }
            else
            {
                var collection = database.GetCollection<BsonDocument>("spotify_db"); ;
                var filter = Builders<BsonDocument>.Filter.Eq("trackid", trackid);
                var state = BsonSerializer.Deserialize<Repository.Track>(collection.Find(filter).First());
                var update = Builders<BsonDocument>.Update.Set("played", state.played++);
                var result = collection.UpdateOneAsync(filter, update).Result;
            }

        }











        static public void FillDB()
        {
            Authentication.ClientId = "94a636ebaa3f46d1af9c8dcf66858daf";
            Authentication.ClientSecret = "8b057cb6a70641969d3a5b706dc501d9";
            Authentication.RedirectUri = "https://65d0c0ea.ngrok.io/auth/";
            var a = Authentication.GetAccessToken("diakov111").Result;
            var Trals = Search("Sport", 2);
            AuthenticationToken token = Auth();
            token.Refresh();

            var user = SpotifyWebAPI.User.GetCurrentUserProfile(token).Result;

            // get this persons playlists
            var playlists = user.GetPlaylists(token).Result;
            var PL = Playlist.GetPlaylist(user.Id, Trals[0].id, token).Result;
        }

        public static playlist[] Search(string keyword, int limit)
        {
            string queryString = "https://api.spotify.com/v1/search?q=%22" + keyword.Replace(" ", "%20") + "%22";
            queryString += "&limit=" + limit;
            queryString += "&type=playlist";

            var json = Get(queryString);
            var obj = JsonConvert.DeserializeObject<playlistsearchresult>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                //TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            return obj.playlists.ToPOCO();

        }

        public static AuthenticationToken Auth()
        {
            var AuthenticationToken = new AuthenticationToken()
            {
                TokenType = "Bearer",
                ExpiresOn = DateTime.Now.AddSeconds(3600),
                RefreshToken = "AQBFdtoxcYKPL_zwCk7PWoCsZIz8wLsai-qSTT05CcJwFMU3o6Xv8MaLJtXM4jj90YFUYz2loooWMRHVGwi39J9-5I8LTcOHz5v30S6DNA7qW2zv6og7ABIxTl85TV9M7CA",
                AccessToken = "BQBGG35Im15rZ3Mop7LKW5YaSoOm-r_8yJdyWiMCSTgbZWk3iLMqkXg38KGFYURzXBj-grvV0qxFqmKM1xL0sC8bfzGsQCbfkGlbnUTkMimqZ2HZvmVgWbE0eGhAcD5j0mIueV7lFYvh0hFfZ3k4PmtFPE7mhA"
            };
            return AuthenticationToken;
        }

        public static string Get(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);

                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                    return reader.ReadToEnd();
            }
        }
        //public static async Task<string> Get(string url, AuthenticationToken token, bool includeBearer = true)
        //{
        //    HttpClient client = new HttpClient();
        //    if (includeBearer)
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        //    else
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.AccessToken);

        //    var httpResponse = await client.GetAsync(url);
        //    return await httpResponse.Content.ReadAsStringAsync();
        //}
        //public static PlaylistTrack[] GetPlaylistTraks(string userId, string playlistId, AuthenticationToken token)
        //{
        //    var json = Get("https://api.spotify.com/v1/users/" + userId + "/playlists/" + playlistId + "/tracks", token).Result;
        //    //var obj = JsonConvert.DeserializeObject<page<playlisttrack>>(json, new JsonSerializerSettings
        //    //{
        //    //    TypeNameHandling = TypeNameHandling.All,
        //    //    //TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
        //    //});

        //    return null; //obj.ToPOCO<PlaylistTrack>();
        //}

    }
}
