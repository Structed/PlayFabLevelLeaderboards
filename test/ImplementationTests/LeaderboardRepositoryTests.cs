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
    public void Test_AddEntry_DoesNotFail()
    {
        var levelId = "level-1";
        var playerId = "player-1";
        var score = 1000;
        leaderboardRepository.AddEntry(levelId, playerId, score);
    }

    [Fact]
    public void Test_GetRankReturnsRank()
    {
        var levelId = "level-1";
        var playerId = "player-1";
        var score = 1000;
        leaderboardRepository.AddEntry(levelId, playerId, score);
        var rank = leaderboardRepository.GetRank(levelId, playerId);
        Assert.NotEqual(0, rank);
    }
}