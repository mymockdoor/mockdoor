using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Headers;

namespace MockDoor.Shared.Models.Response
{
    public class MockResponseDto : IValidatableObject, ICopyTo<MockResponseDto>
    {
        public int Id { get; set; }

        [MaxLength(250, ErrorMessage = "Description too long. Maximum length is 250")]
        public string Description { get; set; }

        public HttpStatusCode Code { get; set; } = HttpStatusCode.OK;

        public SupportedEncodingType Encoding { get; set; } = SupportedEncodingType.UTF8;

        [MaxLength(50, ErrorMessage = "Content type too long. Maximum length is 50")]
        public string ContentType { get; set; } = "text/plain";

        [MaxLength(10_000_000, ErrorMessage = "Response body exceeded max length {0}")]
        public string Body { get; set; }

        [MaxLength(32)]
        public string Checksum { get; set; }

        public int Priority { get; set; } = 100;

        public bool Enabled { get; set; } = true;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public int ServiceRequestId { get; set; }

        public TimeSpan Latency { get; set; }

        public List<MockResponseHeaderDto> Headers { get; set; } = new ();

        public MockResponseDto CopyTo(MockResponseDto target)
        {
            if(target == null)
                throw new NotSupportedException($"{nameof(MockResponseDto)}: Cannot copy to a null target");

            target.Id = Id;
            target.Description = Description;
            target.Code = Code;
            target.Encoding = Encoding;
            target.ContentType = ContentType;
            target.Body = Body;
            target.Checksum = Checksum;
            target.Priority = Priority;
            target.Enabled = Enabled;
            target.ServiceRequestId = ServiceRequestId;
            target.Latency = Latency;
            target.CreatedUtc = CreatedUtc;

            if(target.Headers == null && Headers != null && Headers.Count > 0)
                throw new ArgumentNullException($"{nameof(MockResponseDto)} Headers: Cannot copy to a null target");

            if (Headers != null && Headers.Count > 0)
            {
                if (target.Headers == null)
                {
                    target.Headers = Headers;
                }
                else
                {
                    target.Headers.Clear();
                    target.Headers.AddRange(Headers);
                }
            }
            
            return target;
        }

        public bool IsValid(IDictionary<object, object> validationDictionary = null)
        {
            return GeneralHelper.IsValidFullObject(this, new ValidationContext(this, null, validationDictionary), null);
        }
        
        /// <summary>
        /// Validates tenant
        /// </summary>
        /// <param name="validationContext">a validation context that contains a dictionary of existing tenant paths</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
