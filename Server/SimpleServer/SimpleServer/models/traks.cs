using Newtonsoft.Json;

namespace SimpleServer.models
{
    [JsonObject]
    internal class traks
    {
        public string href { get; set; }
        public int total { get; set; }
    }
}