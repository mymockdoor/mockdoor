using MockDoor.Shared.Models.Microservice;
using System.Collections.Generic;

namespace MockDoor.Shared.Models.ServiceGroup
{
    public class FullServiceGroupDto : BaseServiceGroupDto
    {
        public List<FullMicroserviceDto> Microservices { get; set; }
    }
}
