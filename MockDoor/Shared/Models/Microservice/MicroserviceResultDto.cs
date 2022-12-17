﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Headers;

namespace MockDoor.Shared.Models.Microservice
{
    public class MicroserviceResultDto : IValidatableObject, ICopyTo<MicroserviceResultDto>
    {
        public int Id { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Path is required")]
        [MaxLength(150, ErrorMessage = "field exceeded max length {0}")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Must have alpha numeric characters only")]
        public string Path { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "field exceeded max length {0}")]
        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        public int RegisteredServiceGroupId { get; set; }

        [Url(ErrorMessage = "target url must be a valid url")]
        [MaxLength(250, ErrorMessage = "field exceeded max length {0}")]
        public string TargetUrl { get; set; }

        [Range(0, 6000, ErrorMessage = "Must be between 0 and 6000 milliseconds")]
        public int FakeDelay { get; set; }

        public bool ProxyMode { get; set; } = true;

        public bool RandomiseMockResult { get; set; }

        public DateTime? SimulateTime { get; set; }

        public HeadersMode HeadersMode { get; set; } = HeadersMode.UserDefined;

        public bool PassThroughTenant { get; set; }

        public List<ServiceHeaderDto> Headers { get; set; } = new ();

        public MicroserviceResultDto CopyTo(MicroserviceResultDto target)
        {
            if (target == null)
                throw new NotSupportedException($"{nameof(MicroserviceResultDto)}: Cannot copy to a null target");
            
            target.Id = Id;
            target.Path = Path;
            target.Name = Name;
            target.Enabled = Enabled;
            target.RegisteredServiceGroupId = RegisteredServiceGroupId;
            target.TargetUrl = TargetUrl;
            target.FakeDelay = FakeDelay;
            target.ProxyMode = ProxyMode;
            target.RandomiseMockResult = RandomiseMockResult;
            target.SimulateTime = SimulateTime;
            target.HeadersMode = HeadersMode;
            target.PassThroughTenant = PassThroughTenant;

            target.Headers = new List<ServiceHeaderDto>();
            foreach(var header in Headers)
            {
                target.Headers.Add(header.CopyTo(new ServiceHeaderDto()));
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
            if ((validationContext.MemberName == null || validationContext.MemberName.Equals("Path")) && validationContext.Items.TryGetValue("Path", out object existingPathObjects))
            {
                var existingPaths = (IEnumerable<string>)existingPathObjects;
                if (existingPaths.Any(path => string.Equals(path, Path, StringComparison.CurrentCultureIgnoreCase)))
                {
                    yield return new ValidationResult("Path already taken by existing microservice in this group, please try another", new[] { "Path" });
                }
            }
            if ((validationContext.MemberName == null || validationContext.MemberName.Equals("Name")) && validationContext.Items.TryGetValue("Name", out object existingNameObjects))
            {
                var existingNames = (IEnumerable<string>)existingNameObjects;

                if (existingNames.Any(name => string.Equals(name, Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    yield return new ValidationResult("Name already taken by existing microservice in this group, please try another", new[] { "Name" });
                }
            }

            if (ProxyMode)
            {
                var urlToTest = TargetUrl;
                if (validationContext.Items.TryGetValue("NewPropertyValue", out var overrideTargetUrl))
                {
                    urlToTest = (string)overrideTargetUrl;
                }
                
                if ((validationContext.MemberName == null || validationContext.MemberName.Equals("TargetUrl")) && 
                    string.IsNullOrWhiteSpace(urlToTest) || !Uri.TryCreate(urlToTest, UriKind.Absolute, out var uriResult) ||
                    (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    yield return new ValidationResult("When proxy mode is enabled you must set a TargetUrl", new[] { "TargetUrl" });
                }
            }
        }
    }
}
