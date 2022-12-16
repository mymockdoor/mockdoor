using MockDoor.Shared.Models.Utility;

namespace MockDoor.Abstractions.ConfigurationServices
{
    public interface IDatabaseConfigurationService
    {
        Task<IEnumerable<string>> GetPendingMigrationsAsync();

        IEnumerable<string> GetAllMigrations();

        Task ApplyMigrationsAsync(string connectionString);

        Task<ConnectionStringStatus> DoesConnectionStringWorkAsync(string connectionString);

        Task<ConnectionStringTestResult> TestConnectionStringWorkAsync(string connectionString);
    }
}
