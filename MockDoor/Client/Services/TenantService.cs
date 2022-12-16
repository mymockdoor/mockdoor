using MockDoor.Client.Models;
using MockDoor.Shared.Models.Tenant;
using Radzen;

namespace MockDoor.Client.Services;

public class TenantService : BaseHttpClientService
{
    public TenantService(HttpClient client, NotificationService notificationService) : base(client, notificationService)
    {
    }

    public async Task<HttpServiceResult<TenantListDto>> GetTenantListAsync(int skip = 0, int take = 10)
    {
        var response = await SafeGetAsync($"api/tenant?skip={skip}&take={take}", "An error occured making get tenant request. {0}");
        
        return await HandleResponseAsync<TenantListDto>(response, "Failed to load tenants list");
    }

    public async Task<HttpServiceResult<BaseTenantDto>> GetTenantAsync(int id)
    {
        var response = await SafeGetAsync($"api/tenant/{id}", "An error occured making get tenant request. {0}");
      
        return await HandleResponseAsync<BaseTenantDto>(response, "Tenant not found");
    }

    public async Task<HttpServiceResult<BaseTenantDto>> CreateTenantAsync(BaseTenantDto tenant)
    {
        var response = await SafePostAsync($"api/tenant", tenant, "An error occured making create tenant request. {0}");
        
        return await HandleResponseAsync<BaseTenantDto>(response, "Failed to create", "Successfully created", true);
    }

    public async Task<HttpServiceResult<BaseTenantDto>> UpdateTenantAsync(int id, BaseTenantDto updatedTenant)
    {
        var response = await SafePutAsync($"api/tenant/{id}", updatedTenant, "An error occured making update tenant request. {0}");
       
        return await HandleResponseAsync<BaseTenantDto>(response, "Failed to update", "Successfully updated", true);
    }

    public async Task<bool> DeleteTenantAsync(int id)
    {
        var response = await SafeDeleteAsync($"api/tenant/{id}", "An error occured making delete tenant request. {0}");
        
        var result = await HandleResponseAsync<TenantListDto>(response, "Failed to delete", "Successfully deleted");

        return result.IsSuccessStatusCode;
    }
}