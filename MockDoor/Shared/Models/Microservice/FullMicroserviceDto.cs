using System.Collections.Generic;
using MockDoor.Shared.Models.ServiceRequest;

namespace MockDoor.Shared.Models.Microservice
{
    public class FullMicroserviceDto : MicroserviceResultDto
    {
        public List<ServiceRequestDto> ServiceRequests { get; set; }
    }
}
