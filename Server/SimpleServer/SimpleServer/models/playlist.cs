using SpotifyWebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.models
{
    internal class playlist
    {
        public string name { get; set; }
        public owner owner { get; set; }
        public string id { get; set; }
        public bool? @public { get; set; }
        public string snapshot_id { get; set; }
        public traks tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }

    }
}
