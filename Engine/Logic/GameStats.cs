using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    /// <summary>
    /// A condensed version of GameLogic that may be sent to clients from the server,
    /// so client know the statistics of the game.
    /// </summary>
    public class GameStats : IEncodable
    {
        public String getObjectType()
        {
            return "GameStats";
        }

        #region Teams
        public string LeadingTeam
        {
            get;
            set;
        }
        public int LeadingTeam_NumKills
        {
            get;
            set;
        }
        public int LeadingTeam_NumCaptures
        {
            get;
            set;
        }
        public int LeadingTeam_NumPoints
        {
            get;
            set;
        }

        public string TrailingTeam
        {
            get;
            set;
        }
        public int TrailingTeam_NumKills
        {
            get;
            set;
        }
        public int TrailingTeam_NumCaptures
        {
            get;
            set;
        }
        public int TrailingTeam_NumPoints
        {
            get;
            set;
        }
        #endregion

        #region Time
        public int TimeLeft
        {
            get;
            set;
        }
        #endregion

        #region Player Stats
        private Hashtable Players
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Constructs a GameStats object from the current GameLogic
        /// </summary>
        /// <param name="g">The current GameLogic</param>
        public GameStats(GameLogic g)
        {
            LeadingTeam = g.GetLeadingTeam().ToString();
            LeadingTeam_NumKills = g.GetLeadingTeam().GetKills();
            LeadingTeam_NumCaptures = g.GetLeadingTeam().GetCaptures();
            LeadingTeam_NumPoints = g.GetLeadingTeam().GetTeamPoints();

            TrailingTeam = g.GetTrailingTeam().ToString();
            TrailingTeam_NumKills = g.GetTrailingTeam().GetKills();
            TrailingTeam_NumCaptures = g.GetTrailingTeam().GetCaptures();
            TrailingTeam_NumPoints = g.GetTrailingTeam().GetTeamPoints();

            TimeLeft = g.GetTimeLeft();

            Players = g.GetPlayerStats();
        }

        /// <summary>
        /// Creates a dummy GameStats to be overwritten during decoding
        /// </summary>
        public GameStats()
        {
            LeadingTeam = "";
            LeadingTeam_NumKills = 0;
            LeadingTeam_NumCaptures = 0;
            LeadingTeam_NumPoints = 0;

            TrailingTeam = "";
            TrailingTeam_NumKills = 0;
            TrailingTeam_NumCaptures = 0;
            TrailingTeam_NumPoints = 0;

            TimeLeft = 0;
        }

        /// <summary>
        /// Encodes.
        /// </summary>
        /// <returns>Encoded information.</returns>
        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("LeadingTeam", LeadingTeam);
            e.AddElement("LeadingTeam_NumKills", LeadingTeam_NumKills);
            e.AddElement("LeadingTeam_NumCaptures", LeadingTeam_NumCaptures);
            e.AddElement("LeadingTeam_NumPoints", LeadingTeam_NumPoints);

            e.AddElement("TrailingTeam", TrailingTeam);
            e.AddElement("TrailingTeam_NumKills", TrailingTeam_NumKills);
            e.AddElement("TrailingTeam_NumCaptures", TrailingTeam_NumCaptures);
            e.AddElement("TrailingTeam_NumPoints", TrailingTeam_NumPoints);

            e.AddElement("TimeLeft", TimeLeft);

            return e.Serialize();
        }


        /// <summary>
        /// Decodes.
        /// </summary>
        /// <param name="serialized">Encoded information.</param>
        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            LeadingTeam = (string) e.GetElement("LeadingTeam", LeadingTeam);
            LeadingTeam_NumKills = (int) e.GetElement("LeadingTeam_NumKills", LeadingTeam_NumKills);
            LeadingTeam_NumCaptures = (int)e.GetElement("LeadingTeam_NumCaptures", LeadingTeam_NumCaptures);
            LeadingTeam_NumPoints = (int)e.GetElement("LeadingTeam_NumPoints", LeadingTeam_NumPoints);

            TrailingTeam = (string) e.GetElement("TrailingTeam", TrailingTeam);
            TrailingTeam_NumKills = (int)e.GetElement("TrailingTeam_NumKills", TrailingTeam_NumKills);
            TrailingTeam_NumCaptures = (int)e.GetElement("TrailingTeam_NumCaptures", TrailingTeam_NumCaptures);
            TrailingTeam_NumPoints = (int)e.GetElement("TrailingTeam_NumPoints", TrailingTeam_NumPoints);

            TimeLeft = (int) e.GetElement("TimeLeft", TimeLeft);

            //get string containing player string
            /*string plist = (string)e.GetElement("Players", "");
            //trim leading comma
            string trimplist = plist.Substring(1);
            //define split
            char[] splitat = new char[1];
            splitat[0] = ',';
            string[] splitplist = trimplist.Split(splitat);

            //load to Players
            Players.Clear();
            foreach (string part in splitplist)
            {
                //load dummy player
                PlayerStats p = new PlayerStats();
                //update dummy player
                e.UpdateIEncodable(part, p);
                //get key
                int akey = int.Parse(part);
                //add this to players
                Players.Add(akey, p);
            }*/
        }
    }
}
