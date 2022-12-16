using MockDoor.Shared.Models.ServiceGroup;

namespace MockDoor.Client.Models;

internal class ServiceGroupDeploymentItem : BaseDeploymentItem
{
    public BasicServiceGroupDto ServiceGroup { get; set; }
}