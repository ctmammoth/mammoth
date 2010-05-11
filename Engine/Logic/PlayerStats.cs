using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    /// <summary>
    /// Manages the player's statistics in an Encodable manner.
    /// </summary>
    public class PlayerStats : IEncodable
    {
        public String getObjectType()
        {
            return "PlayerStats";
        }

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

        /// <summary>
        /// Loads the player's current stats server side to be kept and maintained.
        /// </summary>
        /// <param name="nk">Number of kills</param>
        /// <param name="nc">Number of Faptures</param>
        /// <param name="nd">Number of Deaths</param>
        /// <param name="id">Client ID</param>
        /// <param name="g">The current GameLogic</param>
        public PlayerStats(int nk, int nc, int nd, int id, GameLogic g)
        {
            YourTeam = g.GetTeamOf(id);
            NumKills = nk;
            NumCaptures = nc;
            NumDeaths = nd;
        }


        /// <summary>
        /// A dummy constructor which sets up PlayerStats to be overwritten by a Decode
        /// </summary>
        public PlayerStats()
        {
            YourTeam = new Team(1);
            NumKills = 0;
            NumCaptures = 0;
            NumDeaths = 0;
        }


        /// <summary>
        /// Encode.
        /// </summary>
        /// <returns>Encoded information</returns>
        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("YourTeam", YourTeam);
            e.AddElement("NumKills", NumKills);
            e.AddElement("NumCaptures", NumCaptures);
            e.AddElement("NumDeaths", NumDeaths);

            return e.Serialize();
        }

        /// <summary>
        /// Decode.
        /// </summary>
        /// <param name="serialized">Encoded information.</param>
        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            e.UpdateIEncodable("YourTeam", YourTeam);
            NumKills = (int) e.GetElement("NumKills", NumKills);
            NumCaptures = (int) e.GetElement("NumCaptures", NumCaptures);
            NumDeaths = (int) e.GetElement("NumDeaths", NumDeaths);

        }

        /// <summary>
        /// Used for testing.
        /// </summary>
        /// <returns>Any thing you want.</returns>
        public string ToString()
        {
            return "Your Team: " + YourTeam + ", NumKills: " + NumKills;
        }
    }
}
