using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api;

public sealed class ServiceRequestsFunctions(ServiceRequestRepository repository)
{
    [Function(nameof(CreateServiceRequest))]
    public async Task<HttpResponseData> CreateServiceRequest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "service-requests")] HttpRequestData request)
    {
        var serviceRequest = await request.ReadFromJsonAsync<ServiceRequestDto>();
        if (serviceRequest is null)
        {
            return await WriteErrorAsync(request, HttpStatusCode.BadRequest, "Request body is required.");
        }

        var validationErrors = Validate(serviceRequest).ToArray();
        if (validationErrors.Length > 0)
        {
            return await WriteValidationErrorsAsync(request, validationErrors);
        }

        var created = await repository.CreateAsync(serviceRequest);
        var response = request.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Location", $"/api/admin/service-requests/{created.Id}");
        await response.WriteAsJsonAsync(created);
        response.StatusCode = HttpStatusCode.Created;
        return response;
    }

    [Function(nameof(GetServiceRequests))]
    public async Task<HttpResponseData> GetServiceRequests(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/service-requests")] HttpRequestData request)
    {
        var serviceRequests = await repository.GetAllAsync();
        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(serviceRequests);
        response.StatusCode = HttpStatusCode.OK;
        return response;
    }

    [Function(nameof(GetServiceRequestById))]
    public async Task<HttpResponseData> GetServiceRequestById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/service-requests/{id:guid}")] HttpRequestData request,
        Guid id)
    {
        var serviceRequest = await repository.GetByIdAsync(id);
        return serviceRequest is null
            ? await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")
            : await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequest);
    }

    [Function(nameof(UpdateServiceRequestStatus))]
    public async Task<HttpResponseData> UpdateServiceRequestStatus(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/status")] HttpRequestData request,
        Guid id)
    {
        var update = await request.ReadFromJsonAsync<ServiceRequestStatusUpdateModel>();
        if (update is null)
        {
            return await WriteErrorAsync(request, HttpStatusCode.BadRequest, "Request body is required.");
        }

        var validationErrors = Validate(update).ToList();
        if (!ServiceRequestStatuses.All.Contains(update.Status))
        {
            validationErrors.Add("Status must be one of: " + string.Join(", ", ServiceRequestStatuses.All) + ".");
        }

        if (validationErrors.Count > 0)
        {
            return await WriteValidationErrorsAsync(request, validationErrors);
        }

        var serviceRequest = await repository.UpdateStatusAsync(id, update.Status);
        return serviceRequest is null
            ? await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")
            : await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequest);
    }

    [Function(nameof(UpdateServiceRequestQuote))]
    public async Task<HttpResponseData> UpdateServiceRequestQuote(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/quote")] HttpRequestData request,
        Guid id)
    {
        var update = await request.ReadFromJsonAsync<ServiceRequestQuoteUpdateModel>();
        if (update is null)
        {
            return await WriteErrorAsync(request, HttpStatusCode.BadRequest, "Request body is required.");
        }

        var validationErrors = Validate(update).ToArray();
        if (validationErrors.Length > 0)
        {
            return await WriteValidationErrorsAsync(request, validationErrors);
        }

        var serviceRequest = await repository.UpdateQuoteAsync(id, update.EstimateLow, update.EstimateHigh, update.PartsNeeded);
        return serviceRequest is null
            ? await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")
            : await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequest);
    }

    [Function(nameof(UpdateServiceRequestNotes))]
    public async Task<HttpResponseData> UpdateServiceRequestNotes(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/notes")] HttpRequestData request,
        Guid id)
    {
        var update = await request.ReadFromJsonAsync<ServiceRequestNotesUpdateModel>();
        if (update is null)
        {
            return await WriteErrorAsync(request, HttpStatusCode.BadRequest, "Request body is required.");
        }

        var validationErrors = Validate(update).ToArray();
        if (validationErrors.Length > 0)
        {
            return await WriteValidationErrorsAsync(request, validationErrors);
        }

        var serviceRequest = await repository.UpdateNotesAsync(id, update.InternalNotes);
        return serviceRequest is null
            ? await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")
            : await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequest);
    }

    private static IEnumerable<string> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results.Select(result => result.ErrorMessage ?? "Invalid request.");
    }

    private static Task<HttpResponseData> WriteValidationErrorsAsync(HttpRequestData request, IEnumerable<string> errors) =>
        WriteJsonAsync(request, HttpStatusCode.BadRequest, new { errors });

    private static Task<HttpResponseData> WriteErrorAsync(HttpRequestData request, HttpStatusCode statusCode, string message) =>
        WriteJsonAsync(request, statusCode, new { error = message });

    private static async Task<HttpResponseData> WriteJsonAsync<T>(HttpRequestData request, HttpStatusCode statusCode, T body)
    {
        var response = request.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(body);
        response.StatusCode = statusCode;
        return response;
    }
}
