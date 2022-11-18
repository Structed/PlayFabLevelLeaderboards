namespace LevelLeaderboards.Implementation;

public interface ILeaderboardRepository
{
    void AddEntry(string levelId, string titlePlayerId, int score);
    long GetRank(string levelId, string titlePlayerId);
    IList<LeaderboardRepository.LeaderboardEntry> GetTop(string levelId, int count);
}