using StackExchange.Redis;

namespace LevelLeaderboards.Cli
{
    class Program
    {
        static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("level-leaderboards.redis.cache.windows.net:6380,password=Ix5JlphGLPRq77qbSyNNu2bG7xQWLKNeaAzCaNT1guk=,ssl=True,abortConnect=False");
        static async Task Main(string[] args)
        {
            var db = redis.GetDatabase();
            var pong = await db.PingAsync();
            Console.WriteLine(pong);
        }
    }
}

