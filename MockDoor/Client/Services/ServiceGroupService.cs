using MockDoor.Client.Models;
using MockDoor.Shared.Models.ServiceGroup;
using Radzen;

namespace MockDoor.Client.Services
{
    public class ServiceGroupService : BaseHttpClientService
    {
        public ServiceGroupService(HttpClient httpClient, NotificationService notificationService) : base(httpClient, notificationService)
        { }


        public async Task<HttpServiceResult<BasicServiceGroupDto>> GetServiceGroupAsync(int serviceGroupId)
        {
            var response = await SafeGetAsync("api/servicegroup/" + serviceGroupId, "An error occured getting service group. {0}");

            return await HandleResponseAsync<BasicServiceGroupDto>(response, "Service group not found", null, true);
        }

        public async Task<HttpServiceResult<IEnumerable<BaseServiceGroupDto>>> GetServiceGroupListAsync()
        {
            var response = await SafeGetAsync("api/servicegroup/list", "An error occured getting service groups list. {0}");

            return await HandleResponseAsync<IEnumerable<BaseServiceGroupDto>>(response, "Service group list not found", null, true);
        }

        public async Task<HttpServiceResult<ServiceGroupOverviewCollection>> GetServiceGroupsForTenantAsync(int tenantId)
        {
            var response = await SafeGetAsync("api/servicegroup/list/" + tenantId, "An error occured getting service groups list. {0}");

            return await HandleResponseAsync<ServiceGroupOverviewCollection>(response, "Service group list not found", null, true);
        }

        public async Task<HttpServiceResult<BasicServiceGroupDto>> UpdateServiceGroupAsync(int id, BasicServiceGroupDto updatedServiceGroup)
        {
            var response = await SafePutAsync($"api/servicegroup/{id}", updatedServiceGroup, "An error occured making update service group request. {0}");
       
            return await HandleResponseAsync<BasicServiceGroupDto>(response, "Failed to update", "Successfully updated", true);
        }

        public async Task<HttpServiceResult<BasicServiceGroupDto>> CreateServiceGroup(int tenantId, BasicServiceGroupDto serviceGroup)
        {
            var response = await SafePostAsync($"api/servicegroup/{tenantId}", serviceGroup, "An error occured creating service group. {0}");
            
            return await HandleResponseAsync<BasicServiceGroupDto>(response, "Failed to create service group", "Successfully created service group", true);
        }

        public async Task<bool> DeleteServiceGroupAsync(int serviceGroupId)
        {
            var deleteResponse = await SafeDeleteAsync("api/servicegroup/" + serviceGroupId, "An error occured trying to delete service group. {0}");
            
            var response = await HandleResponseAsync<BasicServiceGroupDto>(deleteResponse, "Failed to create request", "Successfully deleted request", true);

            return response.IsSuccessStatusCode;
        }
    }
}
