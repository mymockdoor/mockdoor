using MockDoor.Shared.Models.General;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.Utility;

namespace MockDoor.Abstractions.Repositories
{
    public interface IMicroserviceRepository
    {
        Task<MicroserviceResultDto> CreateMicroservice(MicroserviceResultDto newMicroserviceDto);
        Task<MicroserviceParentIds> GetParentIds(int microserviceId);
        Task<bool> DeleteMicroservice(int id);
        Task<MatchingRequestMicroserviceDetailsDto> FindMatchingRequest(string tenantPath, string serviceGroupPath, string path);
        Task<MicroserviceResultDto> FindMicroservice(string tenantPath, string serviceGroupPath, string path);
        Task<IEnumerable<MicroserviceResultDto>> GetAllMicroserviceForTenant(int tenantId);
        Task<List<PathNameItem>> GetAllMicroservicePathAndNamesForServiceGroup(int serviceGroupId);
        Task<List<PathNameItem>> GetAllMicroservicePathAndNamesForServiceGroup(int serviceGroupId, int excludingMicroserviceId);
        Task<IEnumerable<MicroserviceResultDto>> GetAllMicroservices();
        Task<IEnumerable<MicroserviceSearchResultDto>> GetAllMicroserviceSearchResults();
        Task<IEnumerable<MicroserviceResultDto>> GetAllMicroservicesForServiceGroup(int serviceId);
        Task<MicroserviceResultDto> GetMicroserviceById(int id);
        Task<int?> GetMicroservice(int serviceGroupId, string microservicePath);
        Task<bool> UpdateMicroservice(MicroserviceResultDto updatedMicroservice);
    }
}