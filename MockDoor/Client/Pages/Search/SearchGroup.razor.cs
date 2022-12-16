using Microsoft.AspNetCore.Components;
using MockDoor.Client.Services;
using MockDoor.Shared.Models.ServiceGroup;

namespace MockDoor.Client.Pages.Search;

public partial class SearchGroup
{
    [Inject] public ServiceGroupService ServiceGroupService { get; set; }
    
    List<BaseServiceGroupDto> _serviceGroups;

    protected override async Task OnInitializedAsync()
    {
        var groupsResponse = await ServiceGroupService.GetServiceGroupListAsync();

        if (groupsResponse.IsSuccessStatusCode)
        {
            _serviceGroups = groupsResponse.Content?.ToList();
        }
    }
}