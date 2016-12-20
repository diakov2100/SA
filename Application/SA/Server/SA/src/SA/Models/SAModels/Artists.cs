using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Models.SAModels
{
    //Artist DB structure
    public class Artists
    {
        public string artistid { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public string href { get; set; }
    }
}
