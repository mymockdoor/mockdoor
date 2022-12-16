using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MockDoor.Client.Helpers;
using MockDoor.Client.Models;
using MockDoor.Client.Services;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.ServiceGroup;
using MockDoor.Shared.Models.Tenant;
using MockDoor.Shared.Models.Utility;

namespace MockDoor.Client.Shared.Component.Control;

public partial class MicroserviceSelectorPanel
{
    [Parameter] public int? SelectedTenantId { get; set; }
    
    [Parameter]
    public EventCallback<int?> SelectedTenantIdChanged { get; set; }   
    
    private BaseTenantDto SelectedTenant { get; set; }
    
    [Parameter]
    public EventCallback<BaseTenantDto> SelectedTenantChanged { get; set; }   
    
    [Parameter]
    public int? SelectedServiceGroupId { get; set; }
    
    [Parameter]
    public EventCallback<int?> SelectedServiceGroupIdChanged { get; set; } 
    
    private BasicServiceGroupDto SelectedServiceGroup { get; set; }
    
    [Parameter]
    public EventCallback<BasicServiceGroupDto> SelectedServiceGroupChanged { get; set; } 
    
    [Parameter]
    public int? SelectedMicroserviceId { get; set; }
    
    [Parameter]
    public EventCallback<int?> SelectedMicroserviceIdChanged { get; set; } 

    private MicroserviceResultDto SelectedMicroservice { get; set; }
    
    [Parameter]
    public EventCallback<MicroserviceResultDto> SelectedMicroserviceChanged { get; set; } 
    
    [Parameter] 
    public bool ShowCreateButtons { get; set; }
    
    [Inject] private NavigationManager NavigationManager { get; set; }
    
    [Inject] private TenantService TenantService { get; set; }
    
    [Inject] private ServiceGroupService ServiceGroupService { get; set; }
    
    [Inject] private MicroserviceService MicroserviceService { get; set; }

    private TenantListDto Tenants { get; set; }

    private ServiceGroupOverviewCollection ServiceGroupsCollection { get; set; }

    private List<MicroserviceResultDto> Microservices { get; set; }
    
    private string ReturnUrl
    {
        get
        {
            string returnParams = string.Empty;

            if (SelectedTenantId != null)
            {
                returnParams += $"?tenantId={SelectedTenantId.Value}";
            }

            if (SelectedServiceGroupId != null)
            {
                returnParams += $"&serviceGroupId={SelectedServiceGroupId.Value}";
            }

            if (SelectedMicroserviceId != null)
            {
                returnParams += $"&microserviceId={SelectedMicroserviceId.Value}";
            }

            return System.Web.HttpUtility.UrlEncode(returnParams);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        var tenantResponse = await TenantService.GetTenantListAsync();

        if (tenantResponse.IsSuccessStatusCode)
        {
            Tenants = tenantResponse.Content;

            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("tenantId", out var tenantIdParam))
            {
                await OnChangeTenantAsync(int.Parse(tenantIdParam));
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("serviceGroupId", out var serviceGroupIdParam))
                {
                    await OnChangeServiceGroupAsync(int.Parse(serviceGroupIdParam));

                    if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("microserviceId", out var microserviceIdParam))
                    {
                        await OnChangeMicroserviceAsync(int.Parse(microserviceIdParam.First()));
                    }
                }
            }
            else if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("microserviceId", out var microserviceIdParam))
            {
                int microserviceId = int.Parse(microserviceIdParam.First());

                HttpServiceResult<MicroserviceParentIds> microserviceParentIdsResult = await MicroserviceService.GetMicroserviceParentIdsAsync(microserviceId);

                if (microserviceParentIdsResult.IsSuccessStatusCode)
                {
                    var parentIds = microserviceParentIdsResult.Content;

                    if (parentIds == null)
                    {
                        throw new Exception("parents not found");
                    }

                    await OnChangeTenantAsync(parentIds.TenantId);
                    await OnChangeServiceGroupAsync(parentIds.ServiceGroupId);
                    SelectedMicroserviceId = microserviceId;

                    await OnChangeMicroserviceAsync(microserviceId, true);
                }
            }
        }
    }

    private bool IsTenantSelected()
    {
        return SelectedTenantId != null;
    }

    private bool IsServiceGroupSelected()
    {
        return SelectedTenantId != null;
    }

    async Task OnChangeTenantAsync(int? tenant, bool forceUpdate = false)
    {
        //force update to override with dropdown which will have set id already but not run this logic
        if (SelectedTenantId != tenant || forceUpdate)
        {
            SelectedTenantId = tenant;

            await SelectedTenantIdChanged.InvokeAsync(tenant);

            var newTenant = Tenants?.Tenants?.FirstOrDefault(t => SelectedTenantId != null && t.Id == SelectedTenantId);
            if (SelectedTenant != newTenant)
            {
                SelectedTenant = newTenant;

                await SelectedTenantChanged.InvokeAsync(SelectedTenant);
            }

            if (tenant == null)
            {
                ServiceGroupsCollection = null;

                await OnChangeServiceGroupAsync(null, forceUpdate);

                Microservices = null;

                await OnChangeMicroserviceAsync(null, forceUpdate);
            }
            else
            {
                UpdateQueryParameters();

                var serviceGroupResponse = await ServiceGroupService.GetServiceGroupsForTenantAsync(tenant.Value);

                if (serviceGroupResponse.IsSuccessStatusCode)
                {
                    ServiceGroupsCollection = serviceGroupResponse.Content;

                    await OnChangeServiceGroupAsync(null, forceUpdate);

                    Microservices = null;

                    await OnChangeMicroserviceAsync(null, forceUpdate);
                }
            }
            
            UpdateQueryParameters();
        }
    }

    async Task OnChangeServiceGroupAsync(int? serviceGroupId, bool forceUpdate = false)
    {
        var newServiceGroup = ServiceGroupsCollection?.ServiceGroups?.FirstOrDefault(s =>
            SelectedServiceGroupId != null && s.Id == serviceGroupId);
        
        //force update to override with dropdown which will have set id already but not run this logic
        if (SelectedServiceGroupId != serviceGroupId || forceUpdate)
        {
            if (SelectedServiceGroupId != serviceGroupId)
            {
                SelectedServiceGroupId = serviceGroupId;
                await SelectedServiceGroupIdChanged.InvokeAsync(serviceGroupId);
            }

            if (serviceGroupId == null)
            {
                Microservices = null;
            }
            else
            {
                UpdateQueryParameters();

                var microserviceListResponse =
                    await MicroserviceService.GetMicroserviceListAsync(serviceGroupId.Value);
                
                if(microserviceListResponse.IsSuccessStatusCode)
                {
                    Microservices = microserviceListResponse.Content?.ToList();
                }
            }
                
            SelectedMicroserviceId = null;
            await OnChangeMicroserviceAsync(null, forceUpdate);
            
            UpdateQueryParameters();
        }
        
        if (SelectedServiceGroup != newServiceGroup)
        {
            SelectedServiceGroup = newServiceGroup;
            await SelectedServiceGroupChanged.InvokeAsync(SelectedServiceGroup);
        }
    }

    async Task OnChangeMicroserviceAsync(int? microservice, bool forceUpdate = false)
    {
        //force update to override with dropdown which will have set id already but not run this logic
        if (SelectedMicroserviceId != microservice || forceUpdate)
        {
            SelectedMicroserviceId = microservice;

            await SelectedMicroserviceIdChanged.InvokeAsync(microservice);

            var newMicroservice = Microservices?.FirstOrDefault(p =>
                SelectedMicroserviceId != null && p.Id == SelectedMicroserviceId);

        
            SelectedMicroservice = newMicroservice;
            await SelectedMicroserviceChanged.InvokeAsync(SelectedMicroservice);

            UpdateQueryParameters();
        }
    }

    private void UpdateQueryParameters()
    {
        //update url for bookmarks
        var queryParams = new List<string>();
        string currentUri = NavigationManager.Uri;

        currentUri = currentUri.RemoveQueryString("tenantId");
        if (SelectedTenantId != null)
        {
            queryParams.Add($"tenantId={SelectedTenantId.Value}");
        }

        currentUri = currentUri.RemoveQueryString("serviceGroupId");
        if (SelectedServiceGroupId != null)
        {
            queryParams.Add($"serviceGroupId={SelectedServiceGroupId.Value}");
        }

        currentUri = currentUri.RemoveQueryString("microserviceId");
        if (SelectedMicroserviceId != null)
        {
            queryParams.Add($"microserviceId={SelectedMicroserviceId.Value}");
        }

        NavigationManager.NavigateTo(queryParams.Count > 0
            ? $"{currentUri}?{string.Join("&", queryParams)}"
            : currentUri);
    }
}