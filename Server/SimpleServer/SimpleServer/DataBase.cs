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
                case 2:
                    UserBPM(username, bpm);
                    UpdateTrackInfo(username, 2);
                    return GetTrackID(bpm, username);

            }
            return "";
        }
        static public string GetTrackID(double bpm, string username)
        {
            var collection = database.GetCollection<BsonDocument>("spotify_db");
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Gt("tempo", bpm - 5.0) & filterBuilder.Lte("tempo", bpm + 5.0);
            var statelist = collection.Find(filter).ToList();
            Random rand = new Random();
            var result = BsonSerializer.Deserialize<Repository.Track>(statelist[rand.Next(0, statelist.Count())])
                .trackid;
            UpdateTrackInfo(result, 0);
            SetUserTrack(username, result);
            return result;
        }
        static public void UserBPM(string username, double bpm)
        {
            var collection = database.GetCollection<Repository.User>("users");
            var filter = Builders<Repository.User>.Filter.Eq(s => s.Username, username);
            var state = collection.Find(filter).FirstOrDefaultAsync().Result;
            //var state = BsonSerializer.Deserialize<Repository.User>(collection.Find(filter).FirstOrDefault());
            if (state != null)
            {
                var update = Builders<Repository.User>.Update.Push(s => s.Lastbpm, bpm);
                collection.UpdateOneAsync(filter, update).Wait();
            }
            else
            {
                Repository.User newUser = new Repository.User() { Username = username, Lastbpm = new List<double> { bpm } };
                collection.InsertOneAsync(newUser).Wait();
            }
        }
        static public string GetUserTrack(string username)
        {
            var filter = Builders<Repository.User>.Filter.Eq(s => s.Username, username);
            var collection = database.GetCollection<Repository.User>("Users");
            var state =collection.Find(filter).SingleAsync().Result;
            return state.trackid;
        }
        public static void SetUserTrack(string username, string trackid)
        {
            var filter = Builders<Repository.User>.Filter.Eq(s => s.Username, username);
            var update = Builders<Repository.User>.Update.Set(s => s.trackid, trackid);
            var collection = database.GetCollection<Repository.User>("users");
            collection.UpdateOneAsync(filter, update).Wait();         
        }

        static public void UpdateTrackInfo(string trackid, int fl)
        {
            if (fl == 2)
            {
                var filter = Builders<Repository.Track>.Filter.Eq(s => s.trackid, trackid);
                var update = Builders<Repository.Track>.Update.Inc(s => s.skiped, 1);
                var collection = database.GetCollection<Repository.Track>("spotify_db");
                collection.UpdateOneAsync(filter, update).Wait();
            }
            else
            {
                var filter = Builders<Repository.Track>.Filter.Eq(s => s.trackid, trackid);
                var update = Builders<Repository.Track>.Update.Inc(s => s.played, 1);
                var collection = database.GetCollection<Repository.Track>("spotify_db");
                collection.UpdateOneAsync(filter, update).Wait();
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
