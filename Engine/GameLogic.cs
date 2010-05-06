using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public class GameLogic : Mammoth.Engine.IGameLogic
    {
        public Team team1;
        public Team team2;

        public GameLogic(Game game) : base(game)
        {
            team1 = new Team(1);
            team2 = new Team(2);
        }

        /// <summary>
        /// Adds a client to the smallest team. If teams are same size, adds client to Team 1.
        /// </summary>
        /// <param name="client_id">The ID of the client being added to the team.</param>
        /// <returns>The team the client was added to.</returns>
        public Team AddToTeam(int client_id)
        {
            if (team1.GetTeamSize() > team2.GetTeamSize())
            {
                team2.AddTeamMember(client_id);
                return team2;
            }
            else
            {
                team1.AddTeamMember(client_id);
                return team1;
            }
        }

        /// <summary>
        /// Adds a client to the team with the id number specified.
        /// </summary>
        /// <param name="client_id">The ID of the client being added to the team.</param>
        /// <param name="team_id">The ID of the team to add the client to.</param>
        /// <returns>The team the client was added to.</returns>
        public Team ManuallyAddToTeam(int client_id, int team_id)
        {
            if (team_id == 1)
            {
                team1.AddTeamMember(client_id);
                return team1;
            }
            else if (team_id == 2)
            {
                team2.AddTeamMember(client_id);
                return team2;
            }
            else
            {
                Console.WriteLine("Team " + team_id + " doesn't exist. Cannot add client.");
                return null;
            }
        }

        /// <summary>
        /// Get the Team that is currently winning. If teams are equal in rank, returns Team 1.
        /// </summary>
        /// <returns>Team that is winning.</returns>
        public Team GetLeadingTeam()
        {
            if (team2.GetTeamPoints() <= team1.GetTeamPoints())
                return team1;
            else
                return team2;
        }

        /// <summary>
        /// Get the Team that is currently losing. If teams are equal in rank, returns Team 2.
        /// </summary>
        /// <returns>Team that is winning.</returns>
        public Team GetTrailingTeam()
        {
            if (team2.GetTeamPoints() <= team1.GetTeamPoints())
                return team2;
            else
                return team1;
        }


        /// <summary>
        /// Returns the requested team. Returns null if the team could not be found.
        /// </summary>
        /// <param name="team_id">The ID of the team.</param>
        /// <returns>The team object with the passed in ID.</returns>
        public Team GetTeam(int team_id)
        {
            if (team_id == 1)
                return team1;
            else if (team_id == 2)
                return team2;
            else
            {
                Console.WriteLine("Team " + team_id + " doesn't exist.");
                return null;
            }
        }

        public void AwardKill(int client_id)
        {
            if (team1.GetTeamMemberList().Contains(client_id))
            {
                Console.WriteLine("Awarding kill to Team 1. Thanks to Player " + client_id);
                team1.AddTeamKill();
            }
            else
            {
                Console.WriteLine("Awarding kill to Team 2. Thanks to Player " + client_id);
                team2.AddTeamKill();
            }
        }

    }
}
