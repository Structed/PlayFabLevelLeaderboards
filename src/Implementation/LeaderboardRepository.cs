using System.Reflection.PortableExecutable;
using StackExchange.Redis;

namespace LevelLeaderboards.Implementation;

public class LeaderboardRepository
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

    private RedisKey GetKey(string levelId)
    {
        return $"leaderboard:{levelId}";
    }
}