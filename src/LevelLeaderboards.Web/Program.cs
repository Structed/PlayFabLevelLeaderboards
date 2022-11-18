using LevelLeaderboards.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

var redis = ConnectionMultiplexer.Connect(
    Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? throw new ArgumentNullException(
        "REDIS_CONNECTION_STRING",
        "Environment Variable not set"
    ));
var db = redis.GetDatabase();
var leaderboardRepository = new LeaderboardRepository(redis);

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices( services =>
    {
        services.AddSingleton<ILeaderboardRepository>(leaderboardRepository);
    })
    .Build();

host.Run();