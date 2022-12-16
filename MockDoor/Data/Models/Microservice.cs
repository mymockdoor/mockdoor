using MockDoor.Data.Models.Headers;
using MockDoor.Shared.Models.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MockDoor.Data.Models
{
    public class Microservice
    {
        [Key]
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Path { get; set; }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ServiceGroupID { get; set; }

        [MaxLength(450)]
        public string TargetUrl { get; set; } = string.Empty;

        public int FakeDelay { get; set; }

        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        public bool ProxyMode { get; set; }

        public DateTime? SimulateTime { get; set; }

        public bool RandomiseMockResult { get; set; }

        public bool PassThroughTenant { get; set; }

        public HeadersMode HeadersMode { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ServiceGroup ServiceGroup { get; set; }

        public List<ServiceHeader> Headers { get; set; }

        public List<ServiceRequest> ServiceRequests { get; set; }
    }
}
