using Microsoft.AspNetCore.Components.Authorization;
using Postgrest.Models;

namespace backend.Services;

public class DbService
{
    private Supabase.Client Client { get; }
    private AuthenticationStateProvider CustomAuthStateProvider { get; }
    private ILogger<DbService> Logger { get; }

    public DbService(
        Supabase.Client client,
        AuthenticationStateProvider customAuthStateProvider,
        ILogger<DbService> logger)
    {
        logger.LogInformation("------------------- CONSTRUCTOR -------------------");

        Client = client;
        CustomAuthStateProvider = customAuthStateProvider;
        Logger = logger;
    }

    public async Task<IReadOnlyList<TModel>> From<TModel>() where TModel : BaseModel, new()
    {
        var modeledResponse = await Client
            .From<TModel>()
            .Get();
        return modeledResponse.Models;
    }

    public async Task<List<TModel>> Delete<TModel>(TModel item) where TModel : BaseModel, new()
    {
        var modeledResponse = await Client
            .From<TModel>()
            .Delete(item);
        return modeledResponse.Models;
    }

    public async Task<List<TModel>?> Insert<TModel>(TModel item) where TModel : BaseModel, new()
    {
        Postgrest.Responses.ModeledResponse<TModel> modeledResponse;
        try
        {
            modeledResponse = await Client
                .From<TModel>()
                .Insert(item);			
            
            return modeledResponse.Models;
        }
        catch (Exception ex)
        {
            // if(ex.Response?.StatusCode == HttpStatusCode.Forbidden)
            //     await _dialogService.ShowMessageBox(
            //         "Warning",
            //         "This database request was forbidden."
            //     );
            // else		
            //     await _dialogService.ShowMessageBox(
            //         "Warning",
            //         "This request was not completed because of some problem with the http request. \n "
            //         +ex.Response?.RequestMessage
            //     );
        }
        
        return null;		
    }
}
