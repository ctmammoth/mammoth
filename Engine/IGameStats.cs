using System;
namespace Mammoth.Engine
{
    public interface IGameStats
    {
        string LeadingTeam { get; set; }
        int LeadingTeam_NumCaptures { get; set; }
        int LeadingTeam_NumKills { get; set; }
        int LeadingTeam_NumPoints { get; set; }
        int NumCaptures { get; set; }
        int NumDeaths { get; set; }
        int NumKills { get; set; }
        int TimeLeft { get; set; }
        string TrailingTeam { get; set; }
        int TrailingTeam_NumCaptures { get; set; }
        int TrailingTeam_NumKills { get; set; }
        int TrailingTeam_NumPoints { get; set; }
        string YourTeam { get; set; }
    }
}
