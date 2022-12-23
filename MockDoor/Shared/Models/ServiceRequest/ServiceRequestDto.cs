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
    public class ServiceRequestDto : ICopyTo<ServiceRequestDto>, IValidatableObject
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

        public ServiceRequestDto CopyTo(ServiceRequestDto target)
        {
            target.Id = Id;
            target.FromUrl = FromUrl;
            target.ExactUrlMatch = ExactUrlMatch;
            target.ExpectAuthHeader = ExpectAuthHeader;
            target.MockBehaviour = MockBehaviour;
            target.Enabled = Enabled;
            target.RestType = RestType;
            target.Ttl = Ttl;
            target.FromBody = FromBody;
            target.SimulateTime = SimulateTime;
            target.CreatedUtc = CreatedUtc;
            target.MicroserviceId = MicroserviceId;
            
            target.MockResponses = new List<MockResponseDto>();
            foreach(var response in MockResponses)
            {
                target.MockResponses.Add(response.CopyTo(new MockResponseDto()));
            }
            
            if(target.RequestHeaders == null && RequestHeaders != null && RequestHeaders.Count > 0)
                throw new ArgumentNullException($"{nameof(ServiceRequestDto)} Headers: Cannot copy to a null target");

            if (RequestHeaders != null && RequestHeaders.Count > 0)
            {
                if (target.RequestHeaders == null)
                {
                    target.RequestHeaders = RequestHeaders;
                }
                else
                {
                    target.RequestHeaders.Clear();
                    target.RequestHeaders.AddRange(RequestHeaders);
                }
            }
            
            if(target.QueryParameters == null && QueryParameters != null && QueryParameters.Count > 0)
                throw new ArgumentNullException($"{nameof(ServiceRequestDto)} Query Parameters: Cannot copy to a null target");

            if (QueryParameters != null && QueryParameters.Count > 0)
            {
                if (target.QueryParameters == null)
                {
                    target.QueryParameters = QueryParameters;
                }
                else
                {
                    target.QueryParameters.Clear();
                    target.QueryParameters.AddRange(QueryParameters);
                }
            }

            return target;
        }
    }
}
