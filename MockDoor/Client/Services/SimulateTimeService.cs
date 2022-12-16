using MockDoor.Shared.Models.Timetravel;
using MockDoor.Shared;
using Radzen;
using MockDoor.Client.Models;

namespace MockDoor.Client.Services
{
    public class SimulateTimeService : BaseHttpClientService
    {
        public SimulateTimeService(HttpClient client, NotificationService notificationService) : base(client, notificationService)
        {
        }

        public async Task<HttpServiceResult> SetSimulateTime(int id, DateTime? datetime, TimeTravelScope scope)
        {
            var response = await SafePostAsync($"api/simulatetime/setsimulate/{id}", new UpdateTimeTravelDto() { Time = datetime, Scope = scope }, $"Failed to set simulation time on scope={scope}, id={id}");

            return await HandleResponseAsync(response);
        }

        public async Task<HttpServiceResult<TimeTravelDto>> GetSimulateTimes(TimeTravelScope scope, int id)
        {
            var response = await SafeGetAsync($"api/simulatetime/times/{scope}/{id}", "failed to load simulation time");

            return await HandleResponseAsync<TimeTravelDto>(response, "Failed to load simulation time");
        }
    }
}
