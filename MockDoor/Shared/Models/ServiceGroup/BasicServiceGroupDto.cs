using MockDoor.Shared.Models.Microservice;
using System.Collections.Generic;

namespace MockDoor.Shared.Models.ServiceGroup
{
    public class BasicServiceGroupDto : BaseServiceGroupDto
    {
        public List<MicroserviceResultDto> Microservices { get; set; }
    }
}
