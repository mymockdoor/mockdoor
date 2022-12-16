using System.Collections.Generic;

namespace MockDoor.Shared.Models.ServiceGroup
{
    public class ServiceGroupOverviewCollection
    {
        public int TenantId { get; set; }

        public string TenantName { get; set; }

        public List<BasicServiceGroupDto> ServiceGroups { get; set; }
    }
}
