using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA.Models
{
    public interface ISARepository
    {
        
        string GetTrackID(string username, double bpm, int action);
        bool CheckUser(string username);
        void StartTraining(string username);
        void EndTraining(string username);            
        void UpdateTrackInfo(string username, int action);
        void UpdateUserBPM(string username, double bpm);
        void CheckDBConnection();

    }
}
