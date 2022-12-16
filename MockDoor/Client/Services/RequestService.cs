using Microsoft.AspNetCore.JsonPatch;
using MockDoor.Client.Models;
using MockDoor.Shared.Models.ServiceRequest;
using Radzen;

namespace MockDoor.Client.Services
{
    public class RequestService : BaseHttpClientService
    {
        public RequestService(HttpClient httpClient, NotificationService notificationService) : base(httpClient, notificationService)
        { }

        public async Task<HttpServiceResult<List<ServiceRequestDto>>> GetRequestsAsync(int microserviceId)
        {
            var response = await SafeGetAsync("api/servicerequest/list/" + microserviceId, "An error occured getting microservices. {0}");

            return await HandleResponseAsync<List<ServiceRequestDto>>(response, "Microservice not found", null, true);
        }
        
        public async Task<HttpServiceResult<ServiceRequestDto>> GetRequestAsync(int requestId)
        {
            var response = await SafeGetAsync("api/servicerequest/" + requestId,  "An error occured getting microservice. {0}");

            return await HandleResponseAsync<ServiceRequestDto>(response, "Request not found", null, true);
        }

        public async Task<HttpServiceResult<ServiceRequestDto>> PatchRequestAsync(int requestId, JsonPatchDocument<UpdateServiceRequestDto> patchRequest)
        {
            var response =  await SafePatchAsync("api/servicerequest/" + requestId, patchRequest, "An error occured updating request. {0}");
            
            return await HandleResponseAsync<ServiceRequestDto>(response, "Failed to update request", "Successfully updated request", true);
        }

        public async Task<HttpServiceResult<ServiceRequestDto>> CreateRequest(int microserviceId, ServiceRequestDto request)
        {
            var response = await SafePostAsync($"api/servicerequest/{microserviceId}", request, "An error occured creating request. {0}");
            
            return await HandleResponseAsync<ServiceRequestDto>(response, "Failed to create request", "Successfully created request", true);
        }

        public async Task<bool> DeleteRequestAsync(int requestId)
        {
            var deleteResponse = await SafeDeleteAsync("api/servicerequest/" + requestId, "An error occured trying to delete request. {0}");
            
            var response = await HandleResponseAsync<ServiceRequestDto>(deleteResponse, "Failed to create request", "Successfully deleted request", true);

            return response.IsSuccessStatusCode;
        }
    }
}
