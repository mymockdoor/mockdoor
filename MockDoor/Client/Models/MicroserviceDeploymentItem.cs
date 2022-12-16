using MockDoor.Shared.Models.Microservice;

namespace MockDoor.Client.Models;

internal class MicroserviceDeploymentItem : BaseDeploymentItem
{
    public MicroserviceResultDto Microservice { get; set; }
}