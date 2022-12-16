using MockDoor.Shared.Models.Timetravel;
using MockDoor.Shared.Models.Utility;

namespace MockDoor.Abstractions.Repositories
{
    public interface IBaseRepository
    {
        Task<TimeTravelDto> GetMicroserviceTimes(int id);
        Task<TimeTravelDto> GetRequestTimes(int id);
        Task<TimeTravelDto> GetServiceGroupTimes(int id);
        Task<TimeTravelDto> GetTenantGroupTimes(int id);
        Task<bool> SetSimulateTimeOnMicroservice(DateTime? time, int id);
        Task<bool> SetSimulateTimeOnRequest(DateTime? time, int id);
        Task<bool> SetSimulateTimeOnServiceGroup(DateTime? time, int id);
        Task<bool> SetSimulateTimeOnTenant(DateTime? time, int id);
        Task<FullDatabaseDto> ExportDatabaseToJson();
        Task<bool> ImportDatabase(FullDatabaseDto import, bool skipDuplicateTenants);
    }
}