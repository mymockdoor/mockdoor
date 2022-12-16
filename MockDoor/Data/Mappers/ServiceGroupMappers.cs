using MockDoor.Data.Models;
using MockDoor.Shared.Models.ServiceGroup;

namespace MockDoor.Data.Mappers
{
    public static class ServiceGroupMappers
    {
        #region BasicServiceGroup Mappers
        public static BasicServiceGroupDto ToBasicGroupDto(this ServiceGroup serviceGroup)
        {
            return serviceGroup == null ? null : new BasicServiceGroupDto()
            {
                Id = serviceGroup.ID,
                Enabled = serviceGroup.Enabled,
                DefaultHealthCheckUrl = serviceGroup.DefaultHealthCheckUrl,
                Microservices = serviceGroup.Microservices.ToDtos(serviceGroup.ID),
                Name = serviceGroup.Name,
                Path = serviceGroup.Path,
                TenantId = serviceGroup.TenantID,
                TenantName = serviceGroup.Tenant.Name,
                SimulateTime = serviceGroup.SimulateTime
            };
        }
        #endregion
    }
}
