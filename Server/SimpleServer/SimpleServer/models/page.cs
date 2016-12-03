using Newtonsoft.Json;
using SpotifyWebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.models
{
    [JsonObject]
    internal class page
    {
        public string href { get; set; }
        public playlist[] items { get; set; }
        public int   limit { get; set; }
        public string next{ get; set; }
        public int offset { get; set; }
        public bool? previous { get; set; }
        public int total { get; set; }
        public playlist[] ToPOCO()
        {
            return items;
        }
    }
}