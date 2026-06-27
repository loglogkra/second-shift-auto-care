using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;

namespace SecondShiftAutoCare.Client.Services;

public sealed class StaticWebAppsAuthClient(NavigationManager navigation)
{
    private readonly HttpClient _http = new() { BaseAddress = new Uri(navigation.BaseUri) };

    public bool IsAuthenticated { get; private set; }

    public bool IsAdmin { get; private set; }

    public string? UserName { get; private set; }

    public string? Provider { get; private set; }

    public IReadOnlyList<string> Roles { get; private set; } = [];

    public async Task<AuthState> GetAuthStateAsync()
    {
        try
        {
            var auth = await _http.GetFromJsonAsync<AuthMeResponse>("/.auth/me");
            SetCurrentUser(auth?.ClientPrincipal);
        }
        catch
        {
            SetCurrentUser(null);
        }

        return new AuthState(IsAuthenticated, IsAdmin, UserName, Provider, Roles);
    }

    public async Task<ClientPrincipal?> GetCurrentUserAsync()
    {
        var state = await GetAuthStateAsync();
        return state.IsAuthenticated
            ? new ClientPrincipal
            {
                IdentityProvider = state.Provider,
                UserDetails = state.UserName,
                UserRoles = [.. state.Roles]
            }
            : null;
    }

    private void SetCurrentUser(ClientPrincipal? principal)
    {
        IsAuthenticated = principal is not null;
        UserName = principal?.UserDetails;
        Provider = principal?.IdentityProvider;
        Roles = principal?.UserRoles ?? [];
        IsAdmin = Roles.Any(role => string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase));
    }
}

public sealed record AuthState(
    bool IsAuthenticated,
    bool IsAdmin,
    string? UserName,
    string? Provider,
    IReadOnlyList<string> Roles);

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
