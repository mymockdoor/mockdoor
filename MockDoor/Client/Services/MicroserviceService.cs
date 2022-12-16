using MockDoor.Client.Models;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.Utility;
using Radzen;

namespace MockDoor.Client.Services;

public class MicroserviceService : BaseHttpClientService
{
    public MicroserviceService(HttpClient client, NotificationService notificationService) : base(client, notificationService)
    {
    }

    public async Task<HttpServiceResult<MicroserviceResultDto>> GetMicroserviceAsync(int id)
    {
        var response = await SafeGetAsync($"api/microservice/{id}", "An error occured getting microservice. {0}");

        return await HandleResponseAsync<MicroserviceResultDto>(response, "Microservice not found");
    }

    public async Task<HttpServiceResult<IEnumerable<MicroserviceResultDto>>> GetAllMicroserviceListAsync()
    {
        var response = await SafeGetAsync($"api/microservice/list", "An error occured getting microservice list. {0}");

        return await HandleResponseAsync<IEnumerable<MicroserviceResultDto>>(response, "Microservice list not found");
    }

    public async Task<HttpServiceResult<IEnumerable<MicroserviceSearchResultDto>>> GetMicroserviceSearchResultListAsync()
    {
        var response = await SafeGetAsync($"api/microservice/searchresultlist", "An error occured getting microservice list. {0}");

        return await HandleResponseAsync<IEnumerable<MicroserviceSearchResultDto>>(response, "Microservice list not found");
    }

    public async Task<HttpServiceResult<IEnumerable<MicroserviceResultDto>>> GetMicroserviceListAsync(int id)
    {
        var response = await SafeGetAsync($"api/microservice/list/{id}", "An error occured getting microservice list. {0}");

        return await HandleResponseAsync<IEnumerable<MicroserviceResultDto>>(response, "Microservice not list not found");
    }

    public async Task<HttpServiceResult<MicroserviceResultDto>> CreateMicroserviceAsync(int serviceGroupId, MicroserviceResultDto microservice)
    {
        var response = await SafePostAsync($"api/microservice/{serviceGroupId}", microservice, "An error occured with create microservice request. {0}");
        
        return await HandleResponseAsync<MicroserviceResultDto>(response, "Failed to create", "Successfully created", true);
    }

    public async Task<HttpServiceResult<MicroserviceResultDto>> UpdateMicroserviceAsync(int id, MicroserviceResultDto updatedMicroservice)
    {
        var response = await SafePutAsync($"api/microservice/{id}", updatedMicroservice, "An error occured with update microservice request. {0}");
       
        return await HandleResponseAsync<MicroserviceResultDto>(response, "Failed to update", "Successfully updated", true);
    }

    public async Task<HttpServiceResult<MicroserviceResultDto>> DeleteMicroserviceAsync(int id)
    {
        var response = await SafeDeleteAsync($"api/microservice/{id}", "An error occured making delete microservice request. {0}");
        
        return await HandleResponseAsync<MicroserviceResultDto>(response, "Failed to delete", "Successfully deleted");
    }

    public async Task<HttpServiceResult<MicroserviceParentIds>> GetMicroserviceParentIdsAsync(int microserviceId)
    {
        var response = await SafeGetAsync($"api/microservice/parents/{microserviceId}", "Failed to get microservice parent ids");

        return await HandleResponseAsync<MicroserviceParentIds>(response, "Failed to get ids");
    }
}