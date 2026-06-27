using System.Net.Http.Json;
using System.Net;
using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Client.Services;

public sealed class ServiceRequestClient(HttpClient http)
{
    public async Task<ServiceRequestDto> CreateAsync(ServiceRequestCreateModel model)
    {
        var response = await http.PostAsJsonAsync("api/service-requests", model.ToDto());
        await EnsureSuccessAsync(response, "submit your request");
        return await response.Content.ReadFromJsonAsync<ServiceRequestDto>()
            ?? throw new InvalidOperationException("The service request API returned an empty response.");
    }

    public async Task<List<ServiceRequestDto>> GetAllAsync()
    {
        var response = await http.GetAsync("api/admin/service-requests");
        await EnsureSuccessAsync(response, "load service requests");
        return await response.Content.ReadFromJsonAsync<List<ServiceRequestDto>>() ?? [];
    }

    public async Task<ServiceRequestDto?> GetByIdAsync(Guid id)
    {
        var response = await http.GetAsync($"api/admin/service-requests/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        await EnsureSuccessAsync(response, "load this service request");
        return await response.Content.ReadFromJsonAsync<ServiceRequestDto>();
    }

    public Task<ServiceRequestDto> UpdateStatusAsync(Guid id, ServiceRequestStatusUpdateModel model) =>
        PatchAsync($"api/admin/service-requests/{id}/status", model, "save status");

    public Task<ServiceRequestDto> UpdateQuoteAsync(Guid id, ServiceRequestQuoteUpdateModel model) =>
        PutAsync($"api/admin/service-requests/{id}/quote", model, "save quote");

    public Task<ServiceRequestDto> UpdateNotesAsync(Guid id, ServiceRequestNotesUpdateModel model) =>
        PutAsync($"api/admin/service-requests/{id}/notes", model, "save notes");

    private async Task<ServiceRequestDto> PatchAsync<T>(string uri, T model, string action)
    {
        var response = await http.PutAsJsonAsync(uri, model);
        await EnsureSuccessAsync(response, action);
        return await response.Content.ReadFromJsonAsync<ServiceRequestDto>()
            ?? throw new InvalidOperationException("The service request API returned an empty response.");
    }

    private async Task<ServiceRequestDto> PutAsync<T>(string uri, T model, string action)
    {
        var response = await http.PutAsJsonAsync(uri, model);
        await EnsureSuccessAsync(response, action);
        return await response.Content.ReadFromJsonAsync<ServiceRequestDto>()
            ?? throw new InvalidOperationException("The service request API returned an empty response.");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, string action)
    {
        if (response.IsSuccessStatusCode) return;

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new HttpRequestException("Please sign in as an admin.");
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new HttpRequestException("You are signed in but do not have admin access.");
        }

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Unable to {action}. API returned {(int)response.StatusCode} {response.StatusCode}. {body}");
    }
}
