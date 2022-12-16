using MockDoor.Client.Models;
using MockDoor.Shared.Models.Utility;
using Newtonsoft.Json;
using Radzen;

namespace MockDoor.Client.Services;

public class UtilityService : BaseHttpClientService
{
    public UtilityService(HttpClient client, NotificationService notificationService) : base(client, notificationService)
    {
    }

    public async Task<HttpServiceResult<PingTestResult>> TestUrlAsync(string url, bool pingOnly)
    {
        var response = await SafePostAsync("api/utilities/testurl", new TestUrl() { Url = url, PingOnly = pingOnly }, "Failed to run test request");

        return await HandleResponseAsync<PingTestResult>(response, "Failed to test url");
    }

    public async Task<HttpServiceResult> ImportDatabaseAsJsonAsync(string databaseJson)
    {
        if (string.IsNullOrWhiteSpace(databaseJson))
        {
            return new HttpServiceResult()
            {
                Message = "Invalid database json model"
            };
        }
        
        var databaseDto = JsonConvert.DeserializeObject<FullDatabaseDto>(databaseJson);

        if (databaseDto == null)
        {
            return new HttpServiceResult()
            {
                Message = "Invalid database json model"
            };
        }

        var response = await SafePostAsync("api/utilities/database/import", databaseDto, null);

        return await HandleResponseAsync(response, "Error occured importing database", "Successfully imported database");
    }

    public async Task<HttpServiceResult<string>> ExportAsJsonAsync()
    {
        var response = await SafeGetAsync("api/utilities/database/exportasjson");

        return await HandleResponseAsStringAsync(response, "Error occured exporting database", "Successfully exported database");
    }
}