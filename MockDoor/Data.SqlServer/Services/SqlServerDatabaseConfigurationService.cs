using Microsoft.EntityFrameworkCore;
using MockDoor.Abstractions.ConfigurationServices;
using MockDoor.Data.Contexts;
using MockDoor.Shared.Models.Utility;

namespace Mockdoor.Data.SqlServer.Services
{
    public class SqlServerDatabaseConfigurationService : IDatabaseConfigurationService
    {
        private readonly MockDoorMainContext _mainContext;

        public SqlServerDatabaseConfigurationService(MockDoorMainContext mainContext)
        {
            _mainContext = mainContext;
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
               
                _mainContext.Database.GetDbConnection().ConnectionString = RemoveDatabaseFromSqlConnectionString(connectionString);

                if (await TimeoutConnect())
                {
                    _mainContext.Database.GetDbConnection().ConnectionString = connectionString;
                    return ConnectionStringStatus.ConnectNoDatabase;
                }
                
                return ConnectionStringStatus.Failed;
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

        private static string RemoveDatabaseFromSqlConnectionString(string connectionString)
        {
            string output = connectionString;
            int indexOfDatabase = output.IndexOf(";Database=", StringComparison.Ordinal);
            if (indexOfDatabase > -1)
            {
                int indexOfNextSemicolon = output.IndexOf(';', indexOfDatabase + 1);
                output = output.Remove(indexOfDatabase + 1, indexOfNextSemicolon - indexOfDatabase);
            }
            return output;
        }

        public async Task ApplyMigrationsAsync(string _)
        {
            await _mainContext.Database.MigrateAsync();
        }

        public async Task<ConnectionStringTestResult> TestConnectionStringWorkAsync(string connectionString)
        {
            var result = new ConnectionStringTestResult();

            _mainContext.Database.GetDbConnection().ConnectionString = connectionString;
            try
            {
                if (await _mainContext.Database.CanConnectAsync())
                {
                    result.ConnectionStringStatus = ConnectionStringStatus.Success;
                    result.PendingMigrations = await _mainContext.Database.GetPendingMigrationsAsync();
                    return result;
                }

                _mainContext.Database.GetDbConnection().ConnectionString = RemoveDatabaseFromSqlConnectionString(connectionString);

                if (await _mainContext.Database.CanConnectAsync())
                {
                    _mainContext.Database.GetDbConnection().ConnectionString = connectionString;

                    result.ConnectionStringStatus = ConnectionStringStatus.ConnectNoDatabase;
                    result.Message = "able to connect but no database found";
                    result.PendingMigrations = _mainContext.Database.GetMigrations();
                    return result;
                }

                result.ConnectionStringStatus = ConnectionStringStatus.Failed;
                result.Message = "Enable to connect to database, unknown reason.";
                result.PendingMigrations = _mainContext.Database.GetMigrations();
                return result;
            }
            catch (Exception ex)
            {
                result.ConnectionStringStatus = ConnectionStringStatus.Failed;
                result.Message = ex.Message;
                result.PendingMigrations = _mainContext.Database.GetMigrations();
                return result;
            }
        }

        public IEnumerable<string> GetAllMigrations()
        {
            return _mainContext.Database.GetMigrations();
        }
    }
}
