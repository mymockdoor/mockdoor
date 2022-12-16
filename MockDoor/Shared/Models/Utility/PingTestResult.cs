using MockDoor.Shared.Models.Enum;

namespace MockDoor.Shared.Models.Utility
{
    public class PingTestResult
    {
        public TestUrlResult TestUrlResult { get; set; }

        public int? Latency { get; set; }

        public string Message { get; set; }
    }
}
