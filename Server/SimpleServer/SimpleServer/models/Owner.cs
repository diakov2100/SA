using Newtonsoft.Json;

namespace SimpleServer.models
{
    [JsonObject]
    internal class owner
    {
        public external_urls external_urls { get; set;}
        public string href { get; set; }
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
    }
}