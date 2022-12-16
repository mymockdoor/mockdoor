using MockDoor.Abstractions.MockServices;
using MockDoor.Abstractions.Repositories;
using MockDoor.Shared;
using MockDoor.Shared.Models.Timetravel;

namespace MockDoor.Services.MockServices
{
    public class SimulateTimeService : ISimulateTimeService
    {
        private readonly IBaseRepository _baseRepository;

        public SimulateTimeService(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository ?? throw new ArgumentNullException(nameof(baseRepository));
        }

        public async Task<TimeTravelDto> GetTimes(TimeTravelScope scope, int id)
        {
            switch (scope)
            {
                case TimeTravelScope.Request:       return await _baseRepository.GetRequestTimes(id);
                case TimeTravelScope.Microservice: return await _baseRepository.GetMicroserviceTimes(id);
                case TimeTravelScope.ServiceGroup: return await _baseRepository.GetServiceGroupTimes(id);
                case TimeTravelScope.Tenant: return await _baseRepository.GetTenantGroupTimes(id);
                default: return null; 
            }
        }

        public async Task<bool> SetSimulateTime(UpdateTimeTravelDto updateTimeTravel, int id)
        {
            switch (updateTimeTravel.Scope)
            {
                case TimeTravelScope.Request: return await _baseRepository.SetSimulateTimeOnRequest(updateTimeTravel.Time, id);
                case TimeTravelScope.Microservice: return await _baseRepository.SetSimulateTimeOnMicroservice(updateTimeTravel.Time, id);
                case TimeTravelScope.ServiceGroup: return await _baseRepository.SetSimulateTimeOnServiceGroup(updateTimeTravel.Time, id);
                case TimeTravelScope.Tenant: return await _baseRepository.SetSimulateTimeOnTenant(updateTimeTravel.Time, id);
                default: return false;
            }
        }
    }
}
