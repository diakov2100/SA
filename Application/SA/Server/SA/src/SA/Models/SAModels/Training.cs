using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Models.SAModels
{
    //Training DB structure
    public class Training
    {
        public List<double> bpms;
        public List<string> tracks;
        public DateTime start;
        public DateTime end;
    }
}
