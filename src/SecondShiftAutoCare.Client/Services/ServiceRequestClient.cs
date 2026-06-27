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


    public async Task<List<ServiceIntakeQuestionDto>> GetQuestionsAsync(string serviceType) =>
        await http.GetFromJsonAsync<List<ServiceIntakeQuestionDto>>($"api/service-intake-questions/{Uri.EscapeDataString(serviceType)}") ?? [];
    public async Task<List<JobChecklistItemDto>> GetChecklistAsync(Guid id) => await http.GetFromJsonAsync<List<JobChecklistItemDto>>($"api/admin/service-requests/{id}/checklist") ?? [];
    public async Task<JobChecklistItemDto> AddChecklistItemAsync(Guid id, ChecklistItemCreateModel model) => await ReadAsync<JobChecklistItemDto>(await http.PostAsJsonAsync($"api/admin/service-requests/{id}/checklist", model), "add checklist item");
    public async Task<JobChecklistItemDto> UpdateChecklistItemAsync(Guid id, Guid itemId, ChecklistItemUpdateModel model) => await ReadAsync<JobChecklistItemDto>(await http.PutAsJsonAsync($"api/admin/service-requests/{id}/checklist/{itemId}", model), "update checklist item");
    public async Task DeleteChecklistItemAsync(Guid id, Guid itemId) => await EnsureSuccessAsync(await http.DeleteAsync($"api/admin/service-requests/{id}/checklist/{itemId}"), "delete checklist item");
    public Task<ServiceRequestDto> UpdateScheduleAsync(Guid id, ServiceRequestScheduleUpdateModel model) => PutAsync($"api/admin/service-requests/{id}/schedule", model, "save schedule");
    public async Task<PaymentDto> GetPaymentAsync(Guid id) => await ReadAsync<PaymentDto>(await http.GetAsync($"api/admin/service-requests/{id}/payment"), "load payment");
    public async Task<PaymentDto> UpdatePaymentAsync(Guid id, PaymentDto model) => await ReadAsync<PaymentDto>(await http.PutAsJsonAsync($"api/admin/service-requests/{id}/payment", model), "save payment");
    public async Task<JobRiskAssessmentDto> GetRiskAsync(Guid id) => await ReadAsync<JobRiskAssessmentDto>(await http.GetAsync($"api/admin/service-requests/{id}/risk-assessment"), "load risk assessment");
    public async Task<JobRiskAssessmentDto> UpdateRiskAsync(Guid id, JobRiskAssessmentDto model) => await ReadAsync<JobRiskAssessmentDto>(await http.PutAsJsonAsync($"api/admin/service-requests/{id}/risk-assessment", model), "save risk assessment");
    public async Task<PublicRequestStatusDto?> GetPublicStatusAsync(string token) => await http.GetFromJsonAsync<PublicRequestStatusDto>($"api/request-status/{token}");
    public async Task<PublicQuoteDto?> GetPublicQuoteAsync(string token) => await http.GetFromJsonAsync<PublicQuoteDto>($"api/quote/{token}");
    public async Task<PublicQuoteDto> SaveQuoteDecisionAsync(string token, QuoteDecisionModel model) => await ReadAsync<PublicQuoteDto>(await http.PostAsJsonAsync($"api/quote/{token}/decision", model), "save quote decision");
    public async Task<CustomerHistoryDto?> GetCustomerAsync(Guid id) => await http.GetFromJsonAsync<CustomerHistoryDto>($"api/admin/customers/{id}");
    public async Task<VehicleHistoryDto?> GetVehicleAsync(Guid id) => await http.GetFromJsonAsync<VehicleHistoryDto>($"api/admin/vehicles/{id}");
    private async Task<T> ReadAsync<T>(HttpResponseMessage response, string action) { await EnsureSuccessAsync(response, action); return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("The API returned an empty response."); }

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
