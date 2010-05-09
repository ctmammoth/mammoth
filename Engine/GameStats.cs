﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public class GameStats : IEncodable
    {
        public string YourTeam;
        public int NumKills;
        public int NumCaptures;
        public int NumDeaths;

        public string LeadingTeam;
        public int LeadingTeam_NumKills;
        public int LeadingTeam_NumCaptures;
        public int LeadingTeam_NumPoints;

        public string TrailingTeam;
        public int TrailingTeam_NumKills;
        public int TrailingTeam_NumCaptures;
        public int TrailingTeam_NumPoints;

        public GameStats(int nk, int nc, int nd, int id, IGameLogic g)
        {
            YourTeam = g.GetTeamOf(id).ToString();
            NumKills = nk;
            NumCaptures = nc;
            NumDeaths = nd;

            LeadingTeam = g.GetLeadingTeam().ToString();
            LeadingTeam_NumKills = g.GetLeadingTeam().GetKills();
            LeadingTeam_NumCaptures = g.GetLeadingTeam().GetCaptures();
            LeadingTeam_NumPoints = g.GetLeadingTeam().GetTeamPoints();

            TrailingTeam = g.GetTrailingTeam().ToString();
            TrailingTeam_NumKills = g.GetTrailingTeam().GetKills();
            TrailingTeam_NumCaptures = g.GetTrailingTeam().GetCaptures();
            TrailingTeam_NumPoints = g.GetTrailingTeam().GetTeamPoints();
        }

        public GameStats()
        {
            YourTeam = "";
            NumKills = 0;
            NumCaptures = 0;
            NumDeaths = 0;

            LeadingTeam = "";
            LeadingTeam_NumKills = 0;
            LeadingTeam_NumCaptures = 0;
            LeadingTeam_NumPoints = 0;

            TrailingTeam = "";
            TrailingTeam_NumKills = 0;
            TrailingTeam_NumCaptures = 0;
            TrailingTeam_NumPoints = 0;
        }



        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("YourTeam", YourTeam);
            e.AddElement("NumKills", NumKills);
            e.AddElement("NumCaptures", NumCaptures);
            e.AddElement("NumDeaths", NumDeaths);

            e.AddElement("LeadingTeam", LeadingTeam);
            e.AddElement("LeadingTeam_NumKills", LeadingTeam_NumKills);
            e.AddElement("LeadingTeam_NumCaptures", LeadingTeam_NumCaptures);
            e.AddElement("LeadingTeam_NumPoints", LeadingTeam_NumPoints);

            e.AddElement("TrailingTeam", TrailingTeam);
            e.AddElement("TrailingTeam_NumKills", TrailingTeam_NumKills);
            e.AddElement("TrailingTeam_NumCaptures", TrailingTeam_NumCaptures);
            e.AddElement("TrailingTeam_NumPoints", TrailingTeam_NumPoints);

            return e.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            YourTeam = (string) e.GetElement("YourTeam", YourTeam);
            NumKills = (int) e.GetElement("NumKills", NumKills);
            NumCaptures = (int) e.GetElement("NumCaptures", NumCaptures);
            NumDeaths = (int) e.GetElement("NumDeaths", NumDeaths);

            LeadingTeam = (string) e.GetElement("LeadingTeam", LeadingTeam);
            LeadingTeam_NumKills = (int) e.GetElement("LeadingTeam_NumKills", LeadingTeam_NumKills);
            LeadingTeam_NumCaptures = (int)e.GetElement("LeadingTeam_NumCaptures", LeadingTeam_NumCaptures);
            LeadingTeam_NumPoints = (int)e.GetElement("LeadingTeam_NumPoints", LeadingTeam_NumPoints);

            TrailingTeam = (string) e.GetElement("TrailingTeam", TrailingTeam);
            TrailingTeam_NumKills = (int)e.GetElement("TrailingTeam_NumKills", TrailingTeam_NumKills);
            TrailingTeam_NumCaptures = (int)e.GetElement("TrailingTeam_NumCaptures", TrailingTeam_NumCaptures);
            TrailingTeam_NumPoints = (int)e.GetElement("TrailingTeam_NumPoints", TrailingTeam_NumPoints);
        }


        public string ToString()
        {
            return "Your Team: " + YourTeam + ", NumKills: " + NumKills + ", LeadingTeam: " + LeadingTeam; 
        }
    }
}