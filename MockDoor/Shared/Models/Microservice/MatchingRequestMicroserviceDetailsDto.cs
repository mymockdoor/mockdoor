using MockDoor.Shared.Models.ServiceGroup;
using MockDoor.Shared.Models.Tenant;

namespace MockDoor.Shared.Models.Microservice
{
    public class MatchingRequestMicroserviceDetailsDto
    {
        public MicroserviceResultDto Microservice { get; set; }

        public BaseTenantDto Tenant { get; set; }

        public BasicServiceGroupDto ServiceGroup { get; set; }
    }
}
