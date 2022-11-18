using LevelLeaderboards.Implementation;
using StackExchange.Redis;

namespace ImplementationTests;

public class LeaderboardRepositoryTests
{
    private LeaderboardRepository leaderboardRepository;

    public LeaderboardRepositoryTests()
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING"));
        var db = redis.GetDatabase();
        leaderboardRepository = new LeaderboardRepository(redis);
    }

    [Fact]
    public void Test1()
    {
        var levelId = "level-1";
        var playerId = "player-1";
        var score = 100;
        leaderboardRepository.AddEntry(levelId, playerId, score);
        Assert.True(true);
    }
}