using System.Net;
using LevelLeaderboards.Implementation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace LevelLeaderboards.Web;

public class GetRank
{
    private readonly ILeaderboardRepository leaderboardRepository;

    public GetRank(ILeaderboardRepository leaderboardRepository)
    {
        this.leaderboardRepository = leaderboardRepository;
    }

    [Function("GetRank")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "rank/levelId/{levelId}/titlePlayerId/{titlePlayerId}")] HttpRequestData req, string levelId, string titlePlayerId,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("GetRank");

        HttpResponseData response;
        try
        {
            var rank = leaderboardRepository.GetRank(levelId, titlePlayerId);
            response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteString(rank.ToString());
        }
        catch (NoLeaderboardEntryFoundException e)
        {
            response = req.CreateResponse(HttpStatusCode.NotFound);
            response.WriteString(e.Message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting rank");
            response = req.CreateResponse(HttpStatusCode.InternalServerError);
            response.WriteString(e.Message);
        }

        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        return response;
    }
}