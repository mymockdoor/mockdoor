using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Headers;
using MockDoor.Shared.Models.QueryParameters;
using MockDoor.Shared.Models.Response;

namespace MockDoor.Shared.Models.ServiceRequest
{
    public class UpdateServiceRequestDto
    {
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string FromUrl { get; set; } = string.Empty;

        public bool ExactUrlMatch { get; set; }

        public bool ExpectAuthHeader { get; set; }

        public MockBehaviour MockBehaviour { get; set; }

        public bool Enabled { get; set; } = true;

        public RestType RestType { get; set; }

        public DateTime? SimulateTime { get; set; }

        public TimeSpan? Ttl { get; set; }

        public string FromBody { get; set; }

        public DateTime? CreatedUtc { get; set; }

        public List<MockResponseDto> Responses { get; set; }

        public List<QueryParameterDto> QueryParameters { get; set; }
        
        public List<ServiceRequestHeaderDto> RequestHeaders { get; set; }
    }
}
