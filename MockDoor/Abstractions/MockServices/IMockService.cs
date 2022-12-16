using Microsoft.AspNetCore.Http;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.ServiceRequest;

namespace MockDoor.Abstractions.MockServices
{
    public interface IMockService
    {
        Task CreateMockResponseIfNotExistAsync(MatchingRequestMicroserviceDetailsDto microservice, HttpContext context, RestType restType, string endpointPath, string requestBody, HttpResponseMessage response, TimeSpan latency);

        Task<ServiceRequestDto> FindMatchingServiceRequestAsync(int microserviceId, HttpContext context, RestType restType, string endpoint, string requestBody);
        
        Task<ServiceRequestDto> GetMatchingServiceRequestDtoAsync(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, RestType restType, HttpContext context, string endpointPath);

        Task<MockResponseDto> GetMockResponseAsync(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, RestType restType, HttpContext context, string endpointPath);
    }
}