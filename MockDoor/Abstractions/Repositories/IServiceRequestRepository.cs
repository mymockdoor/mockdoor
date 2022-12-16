using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.ServiceRequest;

namespace MockDoor.Abstractions.Repositories
{
    public interface IServiceRequestRepository
    {
        Task AddResponseToRequestAsync(int serviceRequestId, MockResponseDto mockResponseDto);
        Task<ServiceRequestDto> CreateServiceRequestAsync(int microserviceId, ServiceRequestDto serviceRequestDto);
        Task<bool> DeleteRequest(int serviceRequestId);
        Task<IEnumerable<ServiceRequestDto>> GetAllServiceRequestsForMicroserviceAsync(int microserviceId);
        Task<ServiceRequestDto> GetServiceRequest(int id);
        Task<UpdateServiceRequestDto> GetUpdateServiceRequest(int id);

        Task<DateTime[]> GetMockResponseTimes(int requestId);

        Task<ServiceRequestDto> UpdateMockResponses(int serviceRequestId, List<MockResponseDto> responses);
        Task<UpdateServiceRequestDto> UpdateServiceRequest(int serviceRequestId, UpdateServiceRequestDto serviceRequestDto);
    }
}