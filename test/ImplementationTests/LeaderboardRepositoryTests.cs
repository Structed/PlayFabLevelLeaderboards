using LevelLeaderboards.Implementation;
using StackExchange.Redis;

namespace ImplementationTests;

public class LeaderboardRepositoryTests: IDisposable
{
    private readonly LeaderboardRepository leaderboardRepository;
    private readonly ConnectionMultiplexer redis;

    public LeaderboardRepositoryTests()
    {
        redis = ConnectionMultiplexer.Connect(
            Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? throw new ArgumentNullException(
                "REDIS_CONNECTION_STRING",
                "Environment Variable not set"
        ));
        var db = redis.GetDatabase();
        leaderboardRepository = new LeaderboardRepository(redis);
    }

    // ReSharper disable once UnusedMember.Global
    public void Dispose()
    {
        foreach (var server in redis.GetServers())
        {
            server.FlushDatabase();
        }
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
    public void Test_GetRankWithNoEntriesThrowsException()
    {
        // Arrange
        var levelId = "level-1";
        var playerId = "player-1";

        // Act
        void Action() => leaderboardRepository.GetRank(levelId, playerId);

        // Assert
        Assert.Throws<NoLeaderboardEntryFoundException>((Action)Action);
    }

    [Fact]
    public void Test_GetRankReturnsRank()
    {
        var levelId = "level-1";
        var playerId = "player-1";
        var score = 1000;
        leaderboardRepository.AddEntry(levelId, playerId, score);
        var rank = leaderboardRepository.GetRank(levelId, playerId);
        Assert.NotEqual(1, rank);
    }

     [Fact]
     public void Test_GetTopReturnsAtLeastOneItem()
     {
         var totalEntries = 3;
         var levelId = "level-1";
         var playerIdPrefix = "player-";

         for (var i = 0; i < totalEntries; i++)
         {
             leaderboardRepository.AddEntry(levelId, playerIdPrefix + i, i);
         }

         var top = leaderboardRepository.GetTop(levelId, totalEntries);
         Assert.NotEmpty(top);

         for (var i = totalEntries - 1; i > 0; i--)
         {
             var element = top[i];
             Assert.Equal(element.TitlePlayerId, playerIdPrefix + i);
             Assert.Equal(element.Rank, i);
         }
     }
}