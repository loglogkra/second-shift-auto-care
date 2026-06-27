using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api;

public sealed class ServiceRequestsFunctions(ServiceRequestRepository repository, ILogger<ServiceRequestsFunctions> logger)
{
    private const string AdminRole = "admin";

    [Function(nameof(CreateServiceRequest))]
    public async Task<HttpResponseData> CreateServiceRequest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "service-requests/")] HttpRequestData request)
    {
        logger.LogInformation("POST service-requests route hit.");

        var serviceRequest = await request.ReadFromJsonAsync<ServiceRequestDto>();
        if (serviceRequest is null)
        {
            logger.LogWarning("Service request validation failed: request body was missing.");
            return await WriteErrorAsync(request, HttpStatusCode.BadRequest, "Request body is required.");
        }

        var validationErrors = Validate(serviceRequest).ToList();
        var requestedServices = serviceRequest.ServiceType.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (!requestedServices.Any() || requestedServices.Any(service => !ServiceTypeOptions.All.Contains(service)))
        {
            validationErrors.Add("Service type must include one or more supported services.");
        }

        if (!ServiceRequestUrgencyLevels.All.Contains(serviceRequest.UrgencyLevel))
        {
            validationErrors.Add("Urgency level must be one of: " + string.Join(", ", ServiceRequestUrgencyLevels.All) + ".");
        }

        if (serviceRequest.IsVehicleDrivable is not ("Yes" or "No" or "Unsure"))
        {
            validationErrors.Add("Select whether the vehicle is drivable.");
        }

        if (!serviceRequest.ConsentAccepted)
        {
            validationErrors.Add("Consent/disclaimer acceptance is required.");
        }

        if (validationErrors.Count > 0)
        {
            logger.LogWarning("Service request validation failed with {ValidationErrorCount} error(s).", validationErrors.Count);
            return await WriteValidationErrorsAsync(request, validationErrors);
        }

        return await ExecuteDatabaseActionAsync(request, async () =>
        {
            var created = await repository.CreateAsync(serviceRequest);
            var response = request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Add("Location", $"/api/service-requests/{created.Id}");
            await response.WriteAsJsonAsync(created);
            response.StatusCode = HttpStatusCode.Created;
            return response;
        });
    }

    [Function(nameof(GetServiceRequests))]
    public async Task<HttpResponseData> GetServiceRequests(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/service-requests")] HttpRequestData request)
    {
        logger.LogInformation("Admin service requests route hit: {Method} {Url}.", request.Method, request.Url);
        var unauthorizedResponse = await RequireAdminAsync(request);
        if (unauthorizedResponse is not null)
        {
            return unauthorizedResponse;
        }

        return await ExecuteDatabaseActionAsync(request, async () =>
        {
            var serviceRequests = await repository.GetAllAsync(ReadQuery(request));
            return await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequests);
        });
    }

    [Function(nameof(GetServiceRequestById))]
    public async Task<HttpResponseData> GetServiceRequestById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/service-requests/{id:guid}")] HttpRequestData request,
        Guid id)
    {
        logger.LogInformation("Admin service request detail route hit: {Method} {Url} for request {RequestId}.", request.Method, request.Url, id);
        var unauthorizedResponse = await RequireAdminAsync(request);
        if (unauthorizedResponse is not null)
        {
            return unauthorizedResponse;
        }

        return await ExecuteDatabaseActionAsync(request, async () =>
        {
            var serviceRequest = await repository.GetByIdAsync(id);
            return serviceRequest is null
                ? await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")
                : await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequest);
        });
    }

    [Function(nameof(UpdateServiceRequestStatus))]
    public async Task<HttpResponseData> UpdateServiceRequestStatus(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/status")] HttpRequestData request,
        Guid id)
    {
        logger.LogInformation("Admin service request status route hit: {Method} {Url} for request {RequestId}.", request.Method, request.Url, id);
        var unauthorizedResponse = await RequireAdminAsync(request);
        if (unauthorizedResponse is not null)
        {
            return unauthorizedResponse;
        }

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

        return await ExecuteDatabaseActionAsync(request, async () =>
        {
            var serviceRequest = await repository.UpdateStatusAsync(id, update.Status);
            return serviceRequest is null
                ? await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")
                : await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequest);
        });
    }

    [Function(nameof(UpdateServiceRequestQuote))]
    public async Task<HttpResponseData> UpdateServiceRequestQuote(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/quote")] HttpRequestData request,
        Guid id)
    {
        logger.LogInformation("Admin service request quote route hit: {Method} {Url} for request {RequestId}.", request.Method, request.Url, id);
        var unauthorizedResponse = await RequireAdminAsync(request);
        if (unauthorizedResponse is not null)
        {
            return unauthorizedResponse;
        }

        var update = await request.ReadFromJsonAsync<ServiceRequestQuoteUpdateModel>();
        if (update is null)
        {
            return await WriteErrorAsync(request, HttpStatusCode.BadRequest, "Request body is required.");
        }

        var validationErrors = Validate(update).ToList();
        if (!ServiceRequestApprovalStatuses.All.Contains(update.CustomerApprovalStatus))
        {
            validationErrors.Add("Customer approval status must be one of: " + string.Join(", ", ServiceRequestApprovalStatuses.All) + ".");
        }

        if (validationErrors.Count > 0)
        {
            return await WriteValidationErrorsAsync(request, validationErrors);
        }

        return await ExecuteDatabaseActionAsync(request, async () =>
        {
            var serviceRequest = await repository.UpdateQuoteAsync(id, update);
            return serviceRequest is null
                ? await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")
                : await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequest);
        });
    }

    [Function(nameof(UpdateServiceRequestNotes))]
    public async Task<HttpResponseData> UpdateServiceRequestNotes(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/notes")] HttpRequestData request,
        Guid id)
    {
        logger.LogInformation("Admin service request notes route hit: {Method} {Url} for request {RequestId}.", request.Method, request.Url, id);
        var unauthorizedResponse = await RequireAdminAsync(request);
        if (unauthorizedResponse is not null)
        {
            return unauthorizedResponse;
        }

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

        return await ExecuteDatabaseActionAsync(request, async () =>
        {
            var serviceRequest = await repository.UpdateNotesAsync(id, update.InternalNotes);
            return serviceRequest is null
                ? await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")
                : await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequest);
        });
    }

    [Function(nameof(GetServiceIntakeQuestions))]
    public async Task<HttpResponseData> GetServiceIntakeQuestions([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "service-intake-questions/{serviceType}")] HttpRequestData request, string serviceType) =>
        await ExecuteDatabaseActionAsync(request, async () => await WriteJsonAsync(request, HttpStatusCode.OK, await repository.GetQuestionsAsync(serviceType)));

    [Function(nameof(GetPublicRequestStatus))]
    public async Task<HttpResponseData> GetPublicRequestStatus([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "request-status/{token}")] HttpRequestData request, string token) =>
        await ExecuteDatabaseActionAsync(request, async () => (await repository.GetPublicStatusAsync(token)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Request status was not found."));

    [Function(nameof(GetPublicQuote))]
    public async Task<HttpResponseData> GetPublicQuote([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quote/{token}")] HttpRequestData request, string token) =>
        await ExecuteDatabaseActionAsync(request, async () => (await repository.GetPublicQuoteAsync(token)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Quote was not found."));

    [Function(nameof(SavePublicQuoteDecision))]
    public async Task<HttpResponseData> SavePublicQuoteDecision([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "quote/{token}/decision")] HttpRequestData request, string token)
    { var decision = await request.ReadFromJsonAsync<QuoteDecisionModel>(); if (decision is null) return await WriteErrorAsync(request, HttpStatusCode.BadRequest, "Request body is required."); if (decision.Decision is not (ServiceRequestApprovalStatuses.Approved or ServiceRequestApprovalStatuses.Declined or ServiceRequestApprovalStatuses.Question)) return await WriteErrorAsync(request, HttpStatusCode.BadRequest, "Decision must be Approved, Declined, or Question."); return await ExecuteDatabaseActionAsync(request, async () => (await repository.SaveQuoteDecisionAsync(token, decision)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Quote was not found.")); }

    [Function(nameof(GetChecklist))]
    public async Task<HttpResponseData> GetChecklist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/service-requests/{id:guid}/checklist")] HttpRequestData request, Guid id)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; return await ExecuteDatabaseActionAsync(request, async () => (await repository.GetChecklistAsync(id)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")); }

    [Function(nameof(AddChecklistItem))]
    public async Task<HttpResponseData> AddChecklistItem([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "admin/service-requests/{id:guid}/checklist")] HttpRequestData request, Guid id)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; var model=await request.ReadFromJsonAsync<ChecklistItemCreateModel>(); if(model is null || string.IsNullOrWhiteSpace(model.Text)) return await WriteErrorAsync(request,HttpStatusCode.BadRequest,"Text is required."); return await ExecuteDatabaseActionAsync(request, async () => (await repository.AddChecklistItemAsync(id, model.Text)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")); }

    [Function(nameof(UpdateChecklistItem))]
    public async Task<HttpResponseData> UpdateChecklistItem([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/checklist/{itemId:guid}")] HttpRequestData request, Guid id, Guid itemId)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; var model=await request.ReadFromJsonAsync<ChecklistItemUpdateModel>(); if(model is null) return await WriteErrorAsync(request,HttpStatusCode.BadRequest,"Request body is required."); return await ExecuteDatabaseActionAsync(request, async () => (await repository.UpdateChecklistItemAsync(id, itemId, model.IsCompleted)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Checklist item was not found.")); }

    [Function(nameof(DeleteChecklistItem))]
    public async Task<HttpResponseData> DeleteChecklistItem([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "admin/service-requests/{id:guid}/checklist/{itemId:guid}")] HttpRequestData request, Guid id, Guid itemId)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; return await ExecuteDatabaseActionAsync(request, async () => await WriteJsonAsync(request, (await repository.DeleteChecklistItemAsync(id,itemId)) ? HttpStatusCode.OK : HttpStatusCode.NotFound, new { deleted = true })); }

    [Function(nameof(UpdateSchedule))]
    public async Task<HttpResponseData> UpdateSchedule([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/schedule")] HttpRequestData request, Guid id)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; var model=await request.ReadFromJsonAsync<ServiceRequestScheduleUpdateModel>(); if(model is null) return await WriteErrorAsync(request,HttpStatusCode.BadRequest,"Request body is required."); return await ExecuteDatabaseActionAsync(request, async () => (await repository.UpdateScheduleAsync(id, model)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")); }

    [Function(nameof(GetPayment))]
    public async Task<HttpResponseData> GetPayment([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/service-requests/{id:guid}/payment")] HttpRequestData request, Guid id)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; return await ExecuteDatabaseActionAsync(request, async () => (await repository.GetPaymentAsync(id)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")); }

    [Function(nameof(UpdatePayment))]
    public async Task<HttpResponseData> UpdatePayment([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/payment")] HttpRequestData request, Guid id)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; var model=await request.ReadFromJsonAsync<PaymentDto>(); if(model is null) return await WriteErrorAsync(request,HttpStatusCode.BadRequest,"Request body is required."); return await ExecuteDatabaseActionAsync(request, async () => (await repository.UpdatePaymentAsync(id, model)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")); }

    [Function(nameof(GetRiskAssessment))]
    public async Task<HttpResponseData> GetRiskAssessment([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/service-requests/{id:guid}/risk-assessment")] HttpRequestData request, Guid id)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; return await ExecuteDatabaseActionAsync(request, async () => (await repository.GetRiskAsync(id)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")); }

    [Function(nameof(UpdateRiskAssessment))]
    public async Task<HttpResponseData> UpdateRiskAssessment([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "admin/service-requests/{id:guid}/risk-assessment")] HttpRequestData request, Guid id)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; var model=await request.ReadFromJsonAsync<JobRiskAssessmentDto>(); if(model is null) return await WriteErrorAsync(request,HttpStatusCode.BadRequest,"Request body is required."); return await ExecuteDatabaseActionAsync(request, async () => (await repository.UpdateRiskAsync(id, model)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Service request was not found.")); }

    [Function(nameof(GetCustomerDetail))]
    public async Task<HttpResponseData> GetCustomerDetail([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/customers/{id:guid}")] HttpRequestData request, Guid id)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; return await ExecuteDatabaseActionAsync(request, async () => (await repository.GetCustomerAsync(id)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Customer was not found.")); }

    [Function(nameof(GetVehicleDetail))]
    public async Task<HttpResponseData> GetVehicleDetail([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/vehicles/{id:guid}")] HttpRequestData request, Guid id)
    { var u=await RequireAdminAsync(request); if(u is not null) return u; return await ExecuteDatabaseActionAsync(request, async () => (await repository.GetVehicleAsync(id)) is { } dto ? await WriteJsonAsync(request, HttpStatusCode.OK, dto) : await WriteErrorAsync(request, HttpStatusCode.NotFound, "Vehicle was not found.")); }

    private static ServiceRequestQuery ReadQuery(HttpRequestData request)
    { var q = request.Url.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries).Select(part => part.Split('=', 2)).ToDictionary(parts => Uri.UnescapeDataString(parts[0]), parts => parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty, StringComparer.OrdinalIgnoreCase); string? Get(string key)=>q.TryGetValue(key,out var v)?v:null; return new ServiceRequestQuery{Search=Get("search"),Status=Get("status"),ServiceType=Get("serviceType"),PaymentStatus=Get("paymentStatus"),ScheduledFilter=Get("scheduledFilter"),Sort=Get("sort"),IncludeArchived=bool.TryParse(Get("includeArchived"),out var b)&&b,FromDate=DateTime.TryParse(Get("fromDate"),out var f)?f:null,ToDate=DateTime.TryParse(Get("toDate"),out var t)?t:null}; }


    private async Task<HttpResponseData?> RequireAdminAsync(HttpRequestData request)
    {
        var principal = ReadClientPrincipal(request);
        if (principal?.UserRoles.Any(role => string.Equals(role, AdminRole, StringComparison.OrdinalIgnoreCase)) == true)
        {
            return null;
        }

        logger.LogWarning("Admin API request rejected because x-ms-client-principal did not include the admin role.");
        return await WriteErrorAsync(request, HttpStatusCode.Unauthorized, "Admin role is required.");
    }

    private static StaticWebAppsClientPrincipal? ReadClientPrincipal(HttpRequestData request)
    {
        if (!request.Headers.TryGetValues("x-ms-client-principal", out var values))
        {
            return null;
        }

        var encodedPrincipal = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(encodedPrincipal))
        {
            return null;
        }

        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPrincipal));
            return JsonSerializer.Deserialize<StaticWebAppsClientPrincipal>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }
        catch (FormatException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private async Task<HttpResponseData> ExecuteDatabaseActionAsync(HttpRequestData request, Func<Task<HttpResponseData>> action)
    {
        try
        {
            return await action();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("SqlConnectionString", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogError(ex, "SqlConnectionString is missing from configuration.");
            return await WriteErrorAsync(request, HttpStatusCode.InternalServerError, "The service request database connection is not configured. Set SqlConnectionString in the Function App application settings.");
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Service request database save failed.");
            return await WriteErrorAsync(request, HttpStatusCode.InternalServerError, "We could not save your service request right now. Please try again later.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected service request API error.");
            return await WriteErrorAsync(request, HttpStatusCode.InternalServerError, "Something went wrong while processing the request. Please try again later.");
        }
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


public sealed class StaticWebAppsClientPrincipal
{
    [JsonPropertyName("userRoles")]
    public string[] UserRoles { get; set; } = [];
}
