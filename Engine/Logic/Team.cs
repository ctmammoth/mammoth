﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public class Team
    {
        private int TeamID;
        private List<int> MemberIDs;
        private int NumCaptures;
        private int NumKills;
        private Vector3 SpawnLocation;

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
                SpawnLocation = Vector3.Zero;
            }
            else
            {
                SpawnLocation = Vector3.Zero;
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

        public int GetCaptures()
        {
            return NumCaptures;
        }

        /// <summary>
        /// Adds a kill to the team.
        /// </summary>
        public void AddTeamKill()
        {
            NumKills++;
        }

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

    }
}
