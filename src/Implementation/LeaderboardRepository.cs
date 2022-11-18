using StackExchange.Redis;

namespace LevelLeaderboards.Implementation;

public class LeaderboardRepository : ILeaderboardRepository
{
    private readonly IConnectionMultiplexer connectionMultiplexer;

    public LeaderboardRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        this.connectionMultiplexer = connectionMultiplexer;
    }
    public void AddEntry(string levelId, string titlePlayerId, int score)
    {
        // Fire and forget
        _ = connectionMultiplexer.GetDatabase().SortedSetAddAsync(this.GetKey(levelId), titlePlayerId, score);
    }

    public long GetRank(string levelId, string titlePlayerId)
    {
        var result = connectionMultiplexer.GetDatabase().SortedSetRank(GetKey(levelId), titlePlayerId, Order.Descending);
        var rank = result.GetValueOrDefault();

        return rank;
    }

    public IList<LeaderboardEntry> GetTop(string levelId, int count)
    {
        var result = connectionMultiplexer.GetDatabase().SortedSetRangeByRank(GetKey(levelId), 0, count - 1);
        var entries =  new List<LeaderboardEntry>();

        var rank = 0;
        foreach (var element in result)
        {
            var entry = new LeaderboardEntry
            {
                TitlePlayerId = element,
                Rank = rank
            };
            entries.Add(entry);
            rank++;
        }

        return entries;
    }

    private RedisKey GetKey(string levelId)
    {
        return $"leaderboard:{levelId}";
    }

    public readonly record struct LeaderboardEntry(string TitlePlayerId, long Rank);
}