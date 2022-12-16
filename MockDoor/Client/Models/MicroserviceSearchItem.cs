using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.ServiceGroup;

namespace MockDoor.Client.Models;

public class MicroserviceSearchItem
{
    public MicroserviceResultDto Microservice { get; set; }

    public BaseServiceGroupDto ServiceGroup { get; set; }
}