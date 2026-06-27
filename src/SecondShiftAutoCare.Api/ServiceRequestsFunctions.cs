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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "service-requests")] HttpRequestData request)
    {
        logger.LogInformation("Service request submit started.");

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
            response.Headers.Add("Location", $"/api/admin/service-requests/{created.Id}");
            await response.WriteAsJsonAsync(created);
            response.StatusCode = HttpStatusCode.Created;
            return response;
        });
    }

    [Function(nameof(GetServiceRequests))]
    public async Task<HttpResponseData> GetServiceRequests(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/service-requests")] HttpRequestData request)
    {
        var unauthorizedResponse = await RequireAdminAsync(request);
        if (unauthorizedResponse is not null)
        {
            return unauthorizedResponse;
        }

        return await ExecuteDatabaseActionAsync(request, async () =>
        {
            var serviceRequests = await repository.GetAllAsync();
            return await WriteJsonAsync(request, HttpStatusCode.OK, serviceRequests);
        });
    }

    [Function(nameof(GetServiceRequestById))]
    public async Task<HttpResponseData> GetServiceRequestById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "admin/service-requests/{id:guid}")] HttpRequestData request,
        Guid id)
    {
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
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "admin/service-requests/{id:guid}/status")] HttpRequestData request,
        Guid id)
    {
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
