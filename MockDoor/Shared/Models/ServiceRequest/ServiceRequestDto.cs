using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Headers;
using MockDoor.Shared.Models.QueryParameters;
using MockDoor.Shared.Models.Response;

namespace MockDoor.Shared.Models.ServiceRequest
{
    public class ServiceRequestDto : IValidatableObject
    {
        public int Id { get; set; }
      
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500, ErrorMessage = "Endpoint (From Url) too long. Maximum length is 500")]
        public string FromUrl { get; set; } = string.Empty;

        public bool ExactUrlMatch { get; set; }

        public bool ExpectAuthHeader { get; set; }

        public MockBehaviour MockBehaviour { get; set; }

        public bool Enabled { get; set; } = true;

        public RestType RestType { get; set; }

        public TimeSpan? Ttl { get; set; }
        
        [MaxLength(10_000_000, ErrorMessage = "Body exceeded max length {0}")]
        public string FromBody { get; set; }

        public DateTime? SimulateTime { get; set; }

        public List<MockResponseDto> MockResponses { get; set; }

        public DateTime CreatedUtc { get; set; }

        public int MicroserviceId { get; set; }

        public List<ServiceRequestHeaderDto> RequestHeaders { get; set; }

        public List<QueryParameterDto> QueryParameters { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
