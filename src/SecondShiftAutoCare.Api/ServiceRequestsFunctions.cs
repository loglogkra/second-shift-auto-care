using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api;

public sealed class ServiceRequestsFunctions
{
    [Function(nameof(CreateServiceRequest))]
    public async Task<HttpResponseData> CreateServiceRequest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "requests")] HttpRequestData request)
    {
        var serviceRequest = await request.ReadFromJsonAsync<ServiceRequestDto>();
        var response = request.CreateResponse(HttpStatusCode.Accepted);
        await response.WriteAsJsonAsync(new
        {
            message = "Service request persistence will be implemented with Azure SQL migrations.",
            request = serviceRequest
        });
        return response;
    }

    [Function(nameof(GetServiceRequests))]
    public async Task<HttpResponseData> GetServiceRequests(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "requests")] HttpRequestData request)
    {
        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(Array.Empty<ServiceRequestDto>());
        return response;
    }
}
