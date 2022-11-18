using StackExchange.Redis;

namespace LevelLeaderboards.Cli
{
    class Program
    {
        static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING"));
        static async Task Main(string[] args)
        {
            var db = redis.GetDatabase();
            var pong = await db.PingAsync();
            Console.WriteLine(pong);
        }
    }
}

