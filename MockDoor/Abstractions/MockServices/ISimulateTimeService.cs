using MockDoor.Shared;
using MockDoor.Shared.Models.Timetravel;

namespace MockDoor.Abstractions.MockServices
{
    public interface ISimulateTimeService
    {
        Task<TimeTravelDto> GetTimes(TimeTravelScope scope, int id);
        Task<bool> SetSimulateTime(UpdateTimeTravelDto updateTimeTravel, int id);
    }
}