using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;

namespace SecondShiftAutoCare.Client.Services;

public sealed class StaticWebAppsAuthClient(NavigationManager navigation)
{
    private readonly HttpClient _http = new() { BaseAddress = new Uri(navigation.BaseUri) };

    public async Task<ClientPrincipal?> GetCurrentUserAsync()
    {
        var auth = await _http.GetFromJsonAsync<AuthMeResponse>(".auth/me");
        return auth?.ClientPrincipal;
    }
}

public sealed class AuthMeResponse
{
    [JsonPropertyName("clientPrincipal")]
    public ClientPrincipal? ClientPrincipal { get; set; }
}

public sealed class ClientPrincipal
{
    [JsonPropertyName("identityProvider")]
    public string? IdentityProvider { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("userDetails")]
    public string? UserDetails { get; set; }

    [JsonPropertyName("userRoles")]
    public string[] UserRoles { get; set; } = [];
}
