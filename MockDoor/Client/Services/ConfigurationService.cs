using MockDoor.Client.Models;
using MockDoor.Shared.Models.Configuration;
using MockDoor.Shared.Models.Utility;
using Radzen;

namespace MockDoor.Client.Services;

public class ConfigurationService : BaseHttpClientService
{
    public ConfigurationService(HttpClient client, NotificationService notificationService) : base(client, notificationService)
    {
    }

    public async Task<HttpServiceResult<DeploymentConfiguration>> GetDeploymentConfiguration()
    {
        var response = await SafeGetAsync("api/configuration", "An error occured with getting core configuration. {0}");
      
        return await HandleResponseAsync<DeploymentConfiguration>(response, "Configuration not found");
    }

    public async Task<HttpServiceResult<ConnectionStringTestResult>> TestConnection(string connectionString)
    {
        var response = await SafePostAsync("api/configuration/testconnection", connectionString, "An error occured testing the connection strings. {0}");

        return await HandleResponseAsync<ConnectionStringTestResult>(response, "connection test response not found");
    }

    public async Task<HttpResponseMessage> ApplyMigrationsAsync()
    {
        return await SafePostAsync("api/configuration/applymigrations", "Failed to apply migrations");
    }
}