using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace backend.Services;

class AuthStateProvider: AuthenticationStateProvider
{
    private Supabase.Client Client { get; }
    private ILogger<AuthStateProvider> Logger { get; }

    public AuthStateProvider(
        Supabase.Client client,
        ILogger<AuthStateProvider> logger
    ) {
        logger.LogInformation("------------------- CONSTRUCTOR -------------------");

        Client = client;
        Logger = logger;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Logger.LogInformation("METHOD: GetAuthenticationStateAsync");

        // wait for client to be initialized
        await Client.InitializeAsync();
        var identity = new ClaimsIdentity();
        
        // parse CurrentSession.AccessToken from JWT if not null or empty
        if (!string.IsNullOrEmpty(Client.Auth.CurrentSession?.AccessToken))
        {
            var jwt = ParseClaimsFromJwt(Client.Auth.CurrentSession.AccessToken);
            identity = new ClaimsIdentity(jwt);
        }

        var state = new AuthenticationState(new ClaimsPrincipal(identity));

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }

    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}