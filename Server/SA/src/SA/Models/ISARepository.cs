using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Models
{
    public interface ISARepository
    {
        void UpdateUserBPM(string username, double bpm);
        string GetTrackID(string username, double bpm);
        void UpdateTrackInfo(string username, int action);
        void ConnectDatabase();
    }
}
