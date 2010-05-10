using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public class GameLogic : Mammoth.Engine.IGameLogic
    {
        public Team Team1;
        public Team Team2;
        public DateTime GameStart;
        public bool GameGoing;
        private const int GameLength = 260; //game length in seconds

        /// <summary>
        /// Constructor
        /// </summary>
        public GameLogic()
        {
            ResetGame();
        }

        /// <summary>
        /// Resets game variable such that game has not been initiated.
        /// </summary>
        public void ResetGame()
        {
            //clear all objects
            Team1 = null;
            Team2 = null;
            GameStart = DateTime.Now;
            GameGoing = false;
        }

        /// <summary>
        /// Initializes game logic teams and gameTime.
        /// </summary>
        public void InitiateGame()
        {
            if (GameGoing == false)
            {
                GameGoing = true;

                //declare teams
                Team1 = new Team(1);
                Team2 = new Team(2);

                //initialize gameStart to current time
                GameStart = DateTime.Now;
            }
        }

        /// <summary>
        /// Gets the time left in the game.
        /// </summary>
        /// <returns>The number of seconds left in the game.</returns>
        public int GetTimeLeft()
        {
            TimeSpan diff = DateTime.Now.Subtract(GameStart);
            int timeleft = 260 - diff.Seconds;
            if (timeleft <= 0)
                return 0;
            else
                return timeleft;
        }

        /// <summary>
        /// Adds a client to the smallest team. If teams are same size, adds client to Team 1.
        /// </summary>
        /// <param name="client_id">The ID of the client being added to the team.</param>
        /// <returns>The team the client was added to.</returns>
        public Team AddToTeam(int client_id)
        {
            if (Team1.GetTeamSize() > Team2.GetTeamSize())
            {
                Team2.AddTeamMember(client_id);
                return Team2;
            }
            else
            {
                Team1.AddTeamMember(client_id);
                return Team1;
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
                Team1.AddTeamMember(client_id);
                return Team1;
            }
            else if (team_id == 2)
            {
                Team2.AddTeamMember(client_id);
                return Team2;
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
            if (Team2.GetTeamPoints() <= Team1.GetTeamPoints())
                return Team1;
            else
                return Team2;
        }

        /// <summary>
        /// Get the Team that is currently losing. If teams are equal in rank, returns Team 2.
        /// </summary>
        /// <returns>Team that is winning.</returns>
        public Team GetTrailingTeam()
        {
            if (Team2.GetTeamPoints() <= Team1.GetTeamPoints())
                return Team2;
            else
                return Team1;
        }


        /// <summary>
        /// Returns the requested team. Returns null if the team could not be found.
        /// </summary>
        /// <param name="team_id">The ID of the team.</param>
        /// <returns>The team object with the passed in ID.</returns>
        public Team GetTeam(int team_id)
        {
            if (team_id == 1)
                return Team1;
            else if (team_id == 2)
                return Team2;
            else
            {
                Console.WriteLine("Team " + team_id + " doesn't exist.");
                return null;
            }
        }

        /// <summary>
        /// If someone on a team kills another, give that person's team a point.
        /// </summary>
        /// <param name="client_id">The client id of the killer</param>
        public void AwardKill(int client_id)
        {
            Console.WriteLine("Awarding kill...");

            if (Team1.GetTeamMemberList().Contains(client_id))
            {
                Console.WriteLine("Awarding kill to Team 1. Thanks to Player " + client_id);
                Team1.AddTeamKill();
            }
            else
            {
                Console.WriteLine("Awarding kill to Team 2. Thanks to Player " + client_id);
                Team2.AddTeamKill();
            }
        }


        /// <summary>
        /// Returns the team on which the client indicated resigns.
        /// </summary>
        /// <param name="client_id">The client ID in query</param>
        /// <returns>The team on which that client resides</returns>
        public Team GetTeamOf(int client_id)
        {
            if (Team1.GetTeamMemberList().Contains(client_id))
            {
                return Team1;
            }
            else
            {
                return Team2;
            }
        }
    }
}
