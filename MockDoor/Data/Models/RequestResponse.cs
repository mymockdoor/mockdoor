using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using MockDoor.Data.Models.Headers;
using MockDoor.Shared.Models.Enum;

namespace MockDoor.Data.Models
{
    public class MockResponse
    {
        [Key]
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ID { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        public HttpStatusCode Code { get; set; }

        public SupportedEncodingType Encoding { get; set; } = SupportedEncodingType.UTF8;

        [MaxLength(50)]
        public string ContentType { get; set; } = "text/plain";

        public string Body { get; set; }

        public int ServiceRequestId { get; set; }


        [Required(AllowEmptyStrings = false)]
        [MaxLength(32)]
        [Column(TypeName = "varchar(32)")]
        public string Checksum { get; set; }

        public int Priority { get; set; } = 100;

        public bool Enabled { get; set; } = true;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<ResponseHeader> Headers { get; set; }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ServiceRequest ServiceRequest { get; set; }

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public TimeSpan Latency { get; set; }

        private DateTime _createdUtc = DateTime.Now;
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedUtc
        {
            get
            {
                return _createdUtc;
            }
            set
            {
                _createdUtc = value;
            }
        }
    }
}