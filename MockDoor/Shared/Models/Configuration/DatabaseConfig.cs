using System;

namespace MockDoor.Shared.Models.Configuration
{
    public class DatabaseConfig : ICopyTo<DatabaseConfig>
    {
        public DatabaseProvider Provider { get; set; } = DatabaseProvider.Sqlite;

        public string ConnectionString { get; set; } = "Data Source= mockdoor.db;";
        
        public DatabaseConfig CopyTo(DatabaseConfig target)
        { 
            if (target == null)
                throw new NotSupportedException($"{nameof(DatabaseConfig)}: Cannot copy to a null target");
            
            target.Provider = Provider;
            target.ConnectionString = ConnectionString;
             
            return target;
        }
    }
}
