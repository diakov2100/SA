using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleServer.models
{
    [JsonObject]
    internal class playlistsearchresult
    {
        public page playlists { get; set; }
    }
}
