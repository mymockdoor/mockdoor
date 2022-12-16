using Microsoft.AspNetCore.JsonPatch;
using MockDoor.Client.Models;
using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.ServiceRequest;
using Radzen;

namespace MockDoor.Client.Services
{
    public class ResponseService : BaseHttpClientService
    {
        public ResponseService(HttpClient httpClient, NotificationService notificationService) : base(httpClient, notificationService)
        { }

        public async Task<HttpServiceResult<MockResponseDto>> GetResponseAsync(int responseId)
        {
            var response = await SafeGetAsync("api/mockresponse/" + responseId,  "An error occured getting mock response. {0}");

            return await HandleResponseAsync<MockResponseDto>(response, "Mock response not found", null, true);
        }
        
        public async Task<HttpServiceResult<UpdateMockResponseDto>> PatchResponseAsync(int requestId, JsonPatchDocument<UpdateMockResponseDto> patchRequest)
        {
            var response =  await SafePatchAsync("api/mockresponse/" + requestId, patchRequest, "An error occured updating mock response. {0}");
            
            return await HandleResponseAsync<UpdateMockResponseDto>(response, "Failed to update request", "Successfully updated request", true);
        }

        public async Task<HttpServiceResult<MockResponseDto>> CreateResponseAsync(int requestId, MockResponseDto mockResponse)
        {
            var response = await SafePostAsync("api/mockresponse/" + requestId, mockResponse, $"Failed to create response for request {requestId}");
            
            return await HandleResponseAsync<MockResponseDto>(response, "Failed to update response", "Successfully updated response", true);
        }

        public async Task<HttpServiceResult<ServiceRequestDto>> UpdateResponsesOnRequestAsync(int requestId, IEnumerable<MockResponseDto> mockResponses)
        {
            var response = await SafePutAsync("api/servicerequest/responses/" + requestId, mockResponses, $"Failed to update responses for request {requestId}");
           
            return await HandleResponseAsync<ServiceRequestDto>(response, "Failed to update responses", $"Successfully updated responses for request {requestId}", true);
        }

        public async Task<HttpServiceResult<MockResponseDto>> UpdateResponseOnRequestAsync(int requestId, MockResponseDto mockResponse)
        {
            var response = await SafePutAsync("api/mockresponse/" + requestId, mockResponse,$"Failed to update response for request {requestId}");
           
            return await HandleResponseAsync<MockResponseDto>(response, "Failed to update response", "Successfully updated response", true);
        }

        public async Task<bool> DeleteResponseAsync(int responseId)
        {
            var deleteResponse = await SafeDeleteAsync("api/mockresponse/" + responseId,                 $"Failed to delete mock response {responseId}");
            
            var response = await HandleResponseAsync<MockResponseDto>(deleteResponse, "Failed to create request", "Successfully deleted mock response", true);

            return response.IsSuccessStatusCode;
        }
    }
}
