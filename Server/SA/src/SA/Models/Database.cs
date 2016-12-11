using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Models
{
    public static class Database
    {
        public static MongoClient client;
        public static IMongoDatabase database;
        
        static string connectionString = "mongodb://max:123456@46.101.198.18/SA_db";
        public static void ConnectDataBase()
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("SA_db");
        }
        public static void CheckDBConnection()
        {
            try
            {
                client.ListDatabases();
            }
            catch
            {
                ConnectDataBase();
            }
        }
    }
}
