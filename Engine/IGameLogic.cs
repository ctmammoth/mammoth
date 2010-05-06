using System;
namespace Mammoth.Engine
{
    public interface IGameLogic
    {
        Team AddToTeam(int client_id);
        Team GetLeadingTeam();
        Team GetTeam(int team_id);
        Team GetTrailingTeam();
        Team ManuallyAddToTeam(int client_id, int team_id);
        void AwardKill(int client_id);
    }
}
