using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Mammoth.Engine.Networking;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    /// <summary>
    /// Manages team properties and a list of the IDs of players on the teams.
    /// </summary>
    public class Team : IEncodable
    {
        public String getObjectType()
        {
            return "Team";
        }

        #region Properties
        private int TeamID
        {
            get;
            set;
        }
        private List<int> MemberIDs
        {
            get;
            set;
        }
        private int NumCaptures
        {
            get;
            set;
        }
        private int NumKills
        {
            get;
            set;
        }
        private Vector3 SpawnLocation
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Creates a new team with a unique team number.
        /// </summary>
        /// <param name="id">A unique team number with which to identify the team.</param>
        public Team(int id)
        {
            TeamID = id;
            MemberIDs = new List<int>();
            NumCaptures = 0;
            NumKills = 0;

            if (id == 1)
            {
                SpawnLocation = new Vector3(-3.0f, 10.0f, 0f);
            }
            else
            {
                SpawnLocation = new Vector3(173.0f, -21.0f, -118.0f);
            }
        }


        /// <summary>
        /// Add a client by id to this team.
        /// </summary>
        /// <param name="id">ID of client being added to the team.</param>
        public void AddTeamMember(int id)
        {
            MemberIDs.Add(id);
        }

        /// <summary>
        /// Get the number of people on the team.
        /// </summary>
        public int GetTeamSize()
        {
            return MemberIDs.Count;
        }

        /// <summary>
        /// Get the list of IDs of members on the team.
        /// </summary>
        /// <returns>A list of the ids.</returns>
        public List<int> GetTeamMemberList()
        {
            return MemberIDs;
        }


        /// <summary>
        /// Adds a flag capture point to the team.
        /// </summary>
        public void AddCapture()
        {
            NumCaptures++;
        }

        /// <summary>
        /// Adds a kill to the team.
        /// </summary>
        public void AddTeamKill()
        {
            NumKills++;
        }

        /// <summary>
        /// Get the number of captures the team currently has
        /// </summary>
        /// <returns>The number of captures the team has.</returns>
        public int GetCaptures()
        {
            return NumCaptures;
        }

        /// <summary>
        /// Get the number of kills the team currently has
        /// </summary>
        /// <returns>The number of kills the team has.</returns>
        public int GetKills()
        {
            return NumKills;
        }

        /// <summary>
        /// Gets the team's points. Points are determined by the sum of the number of kills and 10 times the number of flag captures.
        /// </summary>
        public int GetTeamPoints()
        {
            return NumKills + (10 * NumCaptures);
        }

        /// <summary>
        /// Returns the team name as a string.
        /// </summary>
        /// <returns>"Team + (team id)"</returns>
        public string ToString()
        {
            return "Team " + TeamID;
        }

        /// <summary>
        /// Encode.
        /// </summary>
        /// <returns>Encoded information.</returns>
        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("TeamID", TeamID);
            e.AddElement("NumKills", NumKills);
            e.AddElement("NumCaptures", NumCaptures);
            e.AddElement("SpawnLocation", SpawnLocation);

            return e.Serialize();
        }

        /// <summary>
        /// Decode.
        /// </summary>
        /// <param name="serialized">Encoded information.</param>
        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            TeamID = (int)e.GetElement("TeamID", TeamID);
            NumKills = (int)e.GetElement("NumKills", NumKills);
            NumCaptures = (int)e.GetElement("NumCaptures", NumCaptures);
            SpawnLocation = (Vector3)e.GetElement("SpawnLocation", SpawnLocation);
        }

    }
}
