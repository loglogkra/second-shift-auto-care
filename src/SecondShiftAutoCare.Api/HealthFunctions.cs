using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace SecondShiftAutoCare.Api;

public sealed class HealthFunctions(ILogger<HealthFunctions> logger)
{
    [Function(nameof(GetHealth))]
    public async Task<HttpResponseData> GetHealth(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData request)
    {
        logger.LogInformation("Health endpoint hit: {Method} {Url}.", request.Method, request.Url);

        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            status = "ok",
            app = "SecondShiftAutoCare.Api",
            timestampUtc = DateTime.UtcNow
        });
        response.StatusCode = HttpStatusCode.OK;
        return response;
    }
}
