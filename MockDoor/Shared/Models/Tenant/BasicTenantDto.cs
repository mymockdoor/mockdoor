using MockDoor.Shared.Models.ServiceGroup;
using System.Collections.Generic;

namespace MockDoor.Shared.Models.Tenant
{
    public class BasicTenantDto : TenantBase
    {
        public List<BasicServiceGroupDto> ServiceGroups { get; set; }
    }
}
