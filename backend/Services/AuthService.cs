using Microsoft.AspNetCore.Components.Authorization;
using Supabase.Gotrue;

namespace backend.Services;

public class AuthService
{
    private Supabase.Client Client { get; }
    private AuthenticationStateProvider CustomAuthStateProvider { get; }
    private ILogger<AuthService> Logger { get; }

    public AuthService(
        Supabase.Client client,
        AuthenticationStateProvider customAuthStateProvider, 
        ILogger<AuthService> logger
    )
    {
        logger.LogInformation("------------------- CONSTRUCTOR -------------------");

        Client = client;
        CustomAuthStateProvider = customAuthStateProvider;
        Logger = logger;
    }

    public async Task Login(string email, string password)
    {
        Logger.LogInformation("METHOD: Login");
        
        var session = await Client.Auth.SignIn(email, password);
        var currentUser = Client.Auth.CurrentUser;

        Logger.LogInformation("------------------- User logged in -------------------");
        Logger.LogInformation("Id:    {Id}", currentUser?.Id);
        Logger.LogInformation("Email: {Email}", currentUser?.Email);

        await CustomAuthStateProvider.GetAuthenticationStateAsync();
    }
    
    public async Task Logout()
    {
        await Client.Auth.SignOut();
        await CustomAuthStateProvider.GetAuthenticationStateAsync();
    }

    public async Task<User?> GetUser()
    {
        var session = await Client.Auth.RetrieveSessionAsync();
        return session?.User;
    }

}