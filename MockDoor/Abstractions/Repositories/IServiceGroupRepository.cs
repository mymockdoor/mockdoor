using MockDoor.Shared.Models.General;
using MockDoor.Shared.Models.ServiceGroup;

namespace MockDoor.Abstractions.Repositories
{
    public interface IServiceGroupRepository
    {
        Task<BaseServiceGroupDto> CreateServiceGroup(BaseServiceGroupDto newServiceGroupDto);
        Task<bool> DeleteServiceGroup(int id);
        Task<List<PathNameItem>> GetAllServiceGroupNameAndPathsForTenant(int tenantId);
        Task<List<PathNameItem>> GetAllServiceGroupNameAndPathsForTenant(int tenantId, int excludingServiceId);
        Task<BasicServiceGroupDto> GetServiceGroupById(int id);
        Task<int?> GetServiceGroupId(int tenantId, string serviceGroupPath);
        Task<int?> GetServiceGroupId(string path, string serviceGroupPath);
        Task<IEnumerable<BaseServiceGroupDto>> GetServiceGroups();
        Task<ServiceGroupOverviewCollection> GetServiceGroupsByTenantId(int id);
        Task<bool> UpdateServiceGroupBaseValues(BaseServiceGroupDto updatedServiceGroup);
    }
}