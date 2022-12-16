using MockDoor.Data.Models;
using MockDoor.Shared.Models.Microservice;

namespace MockDoor.Data.Mappers
{
    public static class MicroserviceMappers
    {
        public static MicroserviceResultDto ToDto(this Microservice microservice, int serviceGroupId)
        {
            return microservice == null ? null : new MicroserviceResultDto()
            {
                Id = microservice.ID,
                Name = microservice.Name,
                Path = microservice.Path,
                Enabled = microservice.Enabled,
                ProxyMode = microservice.ProxyMode,
                RandomiseMockResult = microservice.RandomiseMockResult,
                FakeDelay = microservice.FakeDelay,
                TargetUrl = microservice.TargetUrl,
                RegisteredServiceGroupId = serviceGroupId,
                SimulateTime = microservice.SimulateTime,
                PassThroughTenant = microservice.PassThroughTenant,
                Headers = microservice.Headers?.ToDtos()
            };
        }

        public static List<MicroserviceResultDto> ToDtos(this List<Microservice> microservice, int serviceGroupId)
        {
            return microservice?.Select(rr => rr.ToDto(serviceGroupId)).ToList();
        }
    }
}
