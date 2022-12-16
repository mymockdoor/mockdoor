using Microsoft.AspNetCore.Components;
using MockDoor.Client.Services;
using MockDoor.Shared.Models.Tenant;

namespace MockDoor.Client.Pages.Search;

public partial class SearchTenant
{
    [Inject] public TenantService TenantService { get; set; }
    
    TenantListDto _tenantList;

    protected override async Task OnInitializedAsync()
    {
        var tenantsResponse = await TenantService.GetTenantListAsync();

        if (tenantsResponse.IsSuccessStatusCode)
        {
            _tenantList = tenantsResponse.Content;
        }
    }
}