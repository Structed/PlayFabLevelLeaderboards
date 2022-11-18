using System.Net;
using LevelLeaderboards.Implementation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LevelLeaderboards.Web;

public class PostScore
{
    private readonly ILeaderboardRepository leaderboardRepository;

    public PostScore(ILeaderboardRepository leaderboardRepository)
    {
        this.leaderboardRepository = leaderboardRepository;
    }

    [Function("PostScore")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "add/levelId/{levelId}/titlePlayerId/{titlePlayerId}/score/{score:int}")] HttpRequestData req, string levelId, string titlePlayerId, int score,
        FunctionContext executionContext)
    {
         this.leaderboardRepository.AddEntry(levelId, titlePlayerId, score);

        var response = req.CreateResponse(HttpStatusCode.OK);

        return response;

    }
}