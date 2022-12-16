using System.Collections.Generic;

namespace MockDoor.Shared.Models.Tenant
{
    public class TenantListDto
    {
        public int TotalTenants { get; set; }

        public List<BaseTenantDto> Tenants { get; set; }
    }
}
