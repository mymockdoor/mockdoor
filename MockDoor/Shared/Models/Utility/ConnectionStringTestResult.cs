﻿using System.Collections.Generic;

namespace MockDoor.Shared.Models.Utility
{
    public class ConnectionStringTestResult
    {
        public ConnectionStringStatus ConnectionStringStatus { get; set; }

        public string Message { get; set; }

        public IEnumerable<string> PendingMigrations { get; set; }
    }
}
