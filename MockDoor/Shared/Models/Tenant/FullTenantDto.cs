using MockDoor.Shared.Models.ServiceGroup;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Headers;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.QueryParameters;
using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.ServiceRequest;

namespace MockDoor.Shared.Models.Tenant
{
    public class FullTenantDto : BaseTenantDto
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<FullServiceGroupDto> ServiceGroups { get; set; }
        
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Id != 0)
            {
                yield return new ValidationResult("Tenant Id must be zero on all tenants in a database backup", new[] { "Tenant.Id" });
            }

            foreach (var groupResult in ValidateGroups(ServiceGroups)) yield return groupResult;

            foreach (var validationResult in base.Validate(validationContext))
            {
                yield return validationResult;
            }
        }

        private IEnumerable<ValidationResult> ValidateGroups(List<FullServiceGroupDto> groups)
        {
            if (groups?.Count > 0)
            {
                foreach (var serviceGroup in groups)
                {
                    if (serviceGroup.Id != 0)
                    {
                        yield return new ValidationResult("Service Group Id must be zero on all tenants in a database backup",
                            new[] { "ServiceGroup.Id" });
                    }
                    
                    foreach (var microserviceResult in ValidateMicroservices(serviceGroup.Microservices)) yield return microserviceResult;
                    
                    var validationResults = new List<ValidationResult>();
                    GeneralHelper.TryValidateFullObject(serviceGroup, new ValidationContext(serviceGroup, null, null), validationResults);
                    foreach (var validationResult in validationResults)
                    {
                        yield return validationResult;
                    }
                }
            }
        }
        
        private IEnumerable<ValidationResult> ValidateMicroservices(List<FullMicroserviceDto> microservices)
        {
            if (microservices?.Count > 0)
            {
                foreach (var microservice in microservices)
                {
                    if (microservice.Id != 0)
                    {
                        yield return new ValidationResult(
                            "Microservice Id must be zero on all tenants in a database backup",
                            new[] { "Microservice.Id" });
                    }

                    foreach (var serviceRequestsResult in ValidateServiceRequests(microservice.ServiceRequests))
                        yield return serviceRequestsResult;

                    var validationResults = new List<ValidationResult>();
                    GeneralHelper.TryValidateFullObject(microservice, new ValidationContext(microservice, null, null), validationResults);
                    foreach (var validationResult in validationResults)
                    {
                        yield return validationResult;
                    }
                }
            }
        }

        private IEnumerable<ValidationResult> ValidateServiceRequests(List<ServiceRequestDto> serviceRequests)
        {
            if (serviceRequests?.Count > 0)
            {
                foreach (var serviceRequest in serviceRequests)
                {
                    if (serviceRequest.Id != 0)
                    {
                        yield return new ValidationResult("Service request Id must be zero on all tenants in a database backup",
                            new[] { "ServiceRequest.Id" });
                    }
                    
                    foreach (var serviceRequestHeaderResults in ValidateServiceRequestHeaders(serviceRequest.RequestHeaders)) yield return serviceRequestHeaderResults;
                    
                    foreach (var serviceRequestQueryParamResults in ValidateServiceRequestQueryParams(serviceRequest.QueryParameters)) yield return serviceRequestQueryParamResults;
                    
                    foreach (var mockResultResults in ValidateMockResponses(serviceRequest.MockResponses)) yield return mockResultResults;
                    
                    var validationResults = new List<ValidationResult>();
                    GeneralHelper.TryValidateFullObject(serviceRequest, new ValidationContext(serviceRequest, null, null), validationResults);
                    foreach (var validationResult in validationResults)
                    {
                        yield return validationResult;
                    }
                }
            }
        }
        
        private IEnumerable<ValidationResult> ValidateServiceRequestHeaders(List<ServiceRequestHeaderDto> serviceRequestHeaders)
        {
            if (serviceRequestHeaders?.Count > 0)
            {
                foreach (var serviceRequestHeader in serviceRequestHeaders)
                {
                    if (serviceRequestHeader.Id != 0)
                    {
                        yield return new ValidationResult("Service request header Id must be zero on all tenants in a database backup",
                            new[] { "ServiceRequestHeader.Id" });
                    }
                }
            }
        }
        
        private IEnumerable<ValidationResult> ValidateServiceRequestQueryParams(List<QueryParameterDto> serviceRequestQueryParams)
        {
            if (serviceRequestQueryParams?.Count > 0)
            {
                foreach (var serviceRequestQueryParam in serviceRequestQueryParams)
                {
                    if (serviceRequestQueryParam.Id != 0)
                    {
                        yield return new ValidationResult("Service request query parameter Id must be zero on all tenants in a database backup",
                            new[] { "ServiceRequestQueryParameter.Id" });
                    }
                }
            }
        }

        private IEnumerable<ValidationResult> ValidateMockResponses(List<MockResponseDto> mockResponses)
        {
            if (mockResponses?.Count > 0)
            {
                foreach (var mockResponse in mockResponses)
                {
                    if (mockResponse.Id != 0)
                    {
                        yield return new ValidationResult("Mock response Id must be zero on all tenants in a database backup",
                            new[] { "MockResponse.Id" });
                    }

                    var validationResults = new List<ValidationResult>();
                    GeneralHelper.TryValidateFullObject(mockResponse, new ValidationContext(mockResponse, null, null), validationResults);
                    foreach (var validationResult in validationResults)
                    {
                        yield return validationResult;
                    }
                }
            }
        }
    }
}
