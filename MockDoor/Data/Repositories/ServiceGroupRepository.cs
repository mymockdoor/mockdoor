using Microsoft.EntityFrameworkCore;
using MockDoor.Abstractions.Repositories;
using MockDoor.Data.Contexts;
using MockDoor.Data.Mappers;
using MockDoor.Data.Models;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.General;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.ServiceGroup;

namespace MockDoor.Data.Repositories
{
    public class ServiceGroupRepository : IServiceGroupRepository
    {
        private readonly MockDoorMainContext _context;

        public ServiceGroupRepository(MockDoorMainContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BaseServiceGroupDto>> GetServiceGroups()
        {
            var services = await _context.ServiceGroups.Include(sg => sg.Tenant).ToListAsync();

            return services.Select(sg =>
                           new BasicServiceGroupDto()
                           {
                               Id = sg.ID,
                               Name = sg.Name,
                               Enabled = sg.Enabled,
                               Path = $"{sg.Path}",
                               TenantId = sg.TenantID,
                               TenantName = sg.Tenant.Name,
                               DefaultHealthCheckUrl = sg.DefaultHealthCheckUrl,
                               Microservices = GetMicroservicesForServiceGroup(sg.ID),
                               SimulateTime = sg.SimulateTime
                           }
                       );
        }

        public async Task<List<PathNameItem>> GetAllServiceGroupNameAndPathsForTenant(int tenantId)
        {
            var serviceGroupPaths = _context.ServiceGroups.Where(sg => sg.TenantID == tenantId)
                .Select(sg => new PathNameItem(sg.Name, sg.Path));

            return await serviceGroupPaths.ToListAsync();
        }

        public async Task<List<PathNameItem>> GetAllServiceGroupNameAndPathsForTenant(int tenantId, int excludingServiceId)
        {
            var serviceGroupPaths = _context.ServiceGroups.Where(sg => sg.TenantID == tenantId && sg.ID != excludingServiceId)
                .Select(sg => new PathNameItem(sg.Name, sg.Path));

            return await serviceGroupPaths.ToListAsync();
        }

        public async Task<ServiceGroupOverviewCollection> GetServiceGroupsByTenantId(int id)
        {
            var tenant = await _context.Tenants.Include(t => t.ServiceGroups).FirstOrDefaultAsync(sg => sg.ID == id);

            if (tenant == null)
            {
                return null;
            }

            if (tenant.ServiceGroups?.Count == 0)
            {
                return new ServiceGroupOverviewCollection()
                {
                    TenantId = id,
                    TenantName = tenant.Name,
                    ServiceGroups = new List<BasicServiceGroupDto>()
                };
            }


            return new ServiceGroupOverviewCollection()
            {
                TenantId = id,
                TenantName = tenant.Name,
                ServiceGroups = tenant.ServiceGroups?.Select(sg =>
                            new BasicServiceGroupDto()
                            {
                                Id = sg.ID,
                                Name = sg.Name,
                                Enabled = sg.Enabled,
                                Path = $"{sg.Path}",
                                TenantId = sg.TenantID,
                                TenantName = sg.Tenant.Name,
                                DefaultHealthCheckUrl = sg.DefaultHealthCheckUrl,
                                Microservices = GetMicroservicesForServiceGroup(sg.ID),
                                SimulateTime = sg.SimulateTime
                            }
                        ).ToList()
            };
        }

        public async Task<BasicServiceGroupDto> GetServiceGroupById(int id)
        {
            var serviceGroup = await _context.ServiceGroups
                                        .Include(sg => sg.Tenant)
                                        .Include(sg => sg.Microservices)
                                        .ThenInclude(ms => ms.ServiceRequests)
                                        .ThenInclude(sr => sr.MockResponses)
                                        .FirstOrDefaultAsync(sg => sg.ID == id);

            return serviceGroup.ToBasicGroupDto();
        }

        public async Task<BaseServiceGroupDto> CreateServiceGroup(BaseServiceGroupDto newServiceGroupDto)
        {
            if (newServiceGroupDto == null)
                throw new Exception("No service group provided");

            if (string.IsNullOrWhiteSpace(newServiceGroupDto.Path))
                throw new Exception("Service group path missing or empty");

            if (string.IsNullOrWhiteSpace(newServiceGroupDto.Name))
                throw new Exception("Service group name missing or empty");

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.ID == newServiceGroupDto.TenantId);

            if (tenant == null)
                throw new Exception("Error no tenant exists for this service");

            var newServiceGroup = new ServiceGroup()
            {
                Name = newServiceGroupDto.Name,
                Path = newServiceGroupDto.Path.ToLower(),
                DefaultHealthCheckUrl = newServiceGroupDto.DefaultHealthCheckUrl,
                TenantID = newServiceGroupDto.TenantId,
                Enabled = newServiceGroupDto.Enabled,
                SimulateTime = newServiceGroupDto.SimulateTime
            };

            _context.ServiceGroups.Add(newServiceGroup);

            await _context.SaveChangesAsync();

            return new BaseServiceGroupDto()
            {
                Id = newServiceGroup.ID,
                Name = newServiceGroup.Name,
                DefaultHealthCheckUrl = newServiceGroup.DefaultHealthCheckUrl,
                Enabled = newServiceGroup.Enabled,
                Path = $"{newServiceGroup.Path}",
                TenantId = newServiceGroup.TenantID,
                SimulateTime = newServiceGroup.SimulateTime
            };
        }

        /// <summary>
        /// Update the base properties ONLY on a service
        /// </summary>
        /// <param name="updatedServiceGroup">the updated service</param>
        /// <returns>true if updated successfully</returns>
        public async Task<bool> UpdateServiceGroupBaseValues(BaseServiceGroupDto updatedServiceGroup)
        {
            var existingServiceGroup = await _context.ServiceGroups.FirstOrDefaultAsync(sg => sg.ID == updatedServiceGroup.Id);

            if (existingServiceGroup == null)
                return false;

            if (string.IsNullOrWhiteSpace(updatedServiceGroup.Path))
                return false;

            existingServiceGroup.Name = updatedServiceGroup.Name;
            existingServiceGroup.DefaultHealthCheckUrl = updatedServiceGroup.DefaultHealthCheckUrl;
            existingServiceGroup.Path = updatedServiceGroup.Path.ToLower();
            existingServiceGroup.Enabled = updatedServiceGroup.Enabled;
            existingServiceGroup.SimulateTime = updatedServiceGroup.SimulateTime;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceGroup(int id)
        {
            var existingService = await _context.ServiceGroups.FirstOrDefaultAsync(sg => sg.ID == id);

            if (existingService == null)
                return false;

            _context.ServiceGroups.Remove(existingService);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int?> GetServiceGroupId(string path, string serviceGroupPath)
        {
            var tenantPathToLower = path.ToLower();
            var serviceGroupPathToLower = serviceGroupPath.ToLower();

            return (await _context.ServiceGroups.FirstOrDefaultAsync(sg => sg.Path == serviceGroupPathToLower && sg.Tenant.Path == tenantPathToLower))?.ID;
        }

        public async Task<int?> GetServiceGroupId(int tenantId, string serviceGroupPath)
        {
            var serviceGroupPathToLower = serviceGroupPath.ToLower();

            return (await _context.ServiceGroups.FirstOrDefaultAsync(sg => sg.Path == serviceGroupPathToLower && sg.Tenant.ID == tenantId))?.ID;
        }

        private List<MicroserviceResultDto> GetMicroservicesForServiceGroup(int serviceGroupId)
        {
            var microservices = _context.Microservices.Include(ms => ms.Headers)
                .Where(pd => pd.ServiceGroupID == serviceGroupId).ToList();

            if (microservices.Count == 0)
                return new List<MicroserviceResultDto>();

            return microservices.Select(ms => new MicroserviceResultDto
            {
                Id = ms.ID,
                Name = ms.Name,
                Enabled = ms.Enabled,
                ProxyMode = ms.ProxyMode,
                RandomiseMockResult = ms.RandomiseMockResult,
                Path = ms.Path,
                FakeDelay = ms.FakeDelay,
                TargetUrl = ms.TargetUrl,
                RegisteredServiceGroupId = ms.ServiceGroupID,
                SimulateTime = ms.SimulateTime,
                HeadersMode = HeadersMode.UserDefined,
                PassThroughTenant = false,
                Headers = ms.Headers?.ToDtos()
            }).ToList();
        }
    }
}
