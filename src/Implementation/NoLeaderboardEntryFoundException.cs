namespace LevelLeaderboards.Implementation;

public class NoLeaderboardEntryFoundException: Exception
{
    private readonly string levelId;
    private readonly string titlePlayerId;

    public NoLeaderboardEntryFoundException(string levelId, string titlePlayerId) : base()
    {
        this.levelId = levelId;
        this.titlePlayerId = titlePlayerId;
    }

    public NoLeaderboardEntryFoundException(string levelId, string titlePlayerId, string message) : base(message)
    {
        this.levelId = levelId;
        this.titlePlayerId = titlePlayerId;
    }
}