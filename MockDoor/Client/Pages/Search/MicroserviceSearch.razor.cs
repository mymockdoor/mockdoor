using Microsoft.AspNetCore.Components;
using MockDoor.Client.Services;
using MockDoor.Shared.Models.Microservice;

namespace MockDoor.Client.Pages.Search;

public partial class MicroserviceSearch
{
    [Inject] public MicroserviceService MicroserviceService { get; set; }
    
    private List<MicroserviceSearchResultDto> _microserviceSearchItems;

    protected override async Task OnInitializedAsync()
    {

        var microservicesResponse = await MicroserviceService.GetMicroserviceSearchResultListAsync();

        if (microservicesResponse.IsSuccessStatusCode)
        {
            _microserviceSearchItems = microservicesResponse.Content?.ToList();
        }
    }
}