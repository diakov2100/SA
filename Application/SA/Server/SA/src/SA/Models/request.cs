using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Models
{
    //POST and PUT request structure 
    public class request
    {
        public double? bpm { get; set; }
        public int? action { get; set; }
        public string username { get; set; }
        public int? style { get; set; }
        public bool is_request_valid()
        {
            if (bpm!=null && action != null && username != null && style != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
