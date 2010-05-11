using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public class PlayerStats : IEncodable
    {
        #region Personal Stats
        public Team YourTeam
        {
            get;
            set;
        }
        public int NumKills
        {
            get;
            set;
        }
        public int NumCaptures
        {
            get;
            set;
        }
        public int NumDeaths
        {
            get;
            set;
        }
        #endregion

        public String getObjectType()
        {
            return "PlayerStats";
        }
 
        public PlayerStats(int nk, int nc, int nd, int id, GameLogic g)
        {
            YourTeam = g.GetTeamOf(id);
            NumKills = nk;
            NumCaptures = nc;
            NumDeaths = nd;
        }

        public PlayerStats()
        {
            YourTeam = new Team(1);
            NumKills = 0;
            NumCaptures = 0;
            NumDeaths = 0;
        }



        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("YourTeam", YourTeam);
            e.AddElement("NumKills", NumKills);
            e.AddElement("NumCaptures", NumCaptures);
            e.AddElement("NumDeaths", NumDeaths);

            return e.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            e.UpdateIEncodable("YourTeam", YourTeam);
            NumKills = (int) e.GetElement("NumKills", NumKills);
            NumCaptures = (int) e.GetElement("NumCaptures", NumCaptures);
            NumDeaths = (int) e.GetElement("NumDeaths", NumDeaths);

        }


        public string ToString()
        {
            return "Your Team: " + YourTeam + ", NumKills: " + NumKills;
        }
    }
}
