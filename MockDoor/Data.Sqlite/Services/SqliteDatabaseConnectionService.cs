using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MockDoor.Abstractions.ConfigurationServices;
using MockDoor.Data.Contexts;
using MockDoor.Shared.Models.Utility;

namespace Mockdoor.Data.Sqlite.Services
{
    public class SqliteDatabaseConnectionService : IDatabaseConfigurationService
    {
        private readonly MockDoorMainContext _mainContext;

        public SqliteDatabaseConnectionService(MockDoorMainContext mainContext)
        {
            _mainContext = mainContext ?? throw new ArgumentNullException(nameof(mainContext));
        }

        public async Task<IEnumerable<string>> GetPendingMigrationsAsync()
        {
            return await _mainContext.Database.GetPendingMigrationsAsync();
        }

        public async Task<ConnectionStringStatus> DoesConnectionStringWorkAsync(string connectionString)
        {
            _mainContext.Database.GetDbConnection().ConnectionString = connectionString;
            try
            {
                if (await TimeoutConnect())
                    return ConnectionStringStatus.Success;

                return ConnectionStringStatus.ConnectNoDatabase;
            }
            catch (Exception)
            {
                return ConnectionStringStatus.Failed;
            }
        }

        private async Task<bool> TimeoutConnect()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                return await _mainContext.Database.CanConnectAsync(cancellationTokenSource.Token);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task ApplyMigrationsAsync(string connectionString)
        {
            var sqliteFilePath = GetFilePathFromConnectionString(connectionString);

            if (!string.IsNullOrWhiteSpace(sqliteFilePath))
            {
                // check directory exists
                if (!Directory.Exists(sqliteFilePath))
                {
                    Directory.CreateDirectory(sqliteFilePath);
                }
            }
            
            await _mainContext.Database.MigrateAsync();
        }

        private string GetFilePathFromConnectionString(string connectionString)
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;
            string filePath = ((string)builder["Data Source"]).Trim();

            return Path.GetDirectoryName(filePath);
        }

        public async Task<ConnectionStringTestResult> TestConnectionStringWorkAsync(string connectionString)
        {
            var result = new ConnectionStringTestResult();

            result.ConnectionStringStatus = ConnectionStringStatus.Success;
            result.PendingMigrations = await _mainContext.Database.GetPendingMigrationsAsync();
            return result;
        }

        public IEnumerable<string> GetAllMigrations()
        {
            return _mainContext.Database.GetMigrations();
        }
    }
}
