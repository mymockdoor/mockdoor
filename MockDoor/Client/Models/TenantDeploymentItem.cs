using MockDoor.Shared.Models.Tenant;

namespace MockDoor.Client.Models;

internal class TenantDeploymentItem : BaseDeploymentItem
{
    public BaseTenantDto Tenant { get; set; }

    public ServiceGroupDeploymentItem ServiceGroupDeploymentItem { get; set; }

    public List<MicroserviceDeploymentItem> MicroserviceDeployments { get; set; } = new();
}