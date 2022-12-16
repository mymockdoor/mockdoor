using MockDoor.Data.Helpers;
using MockDoor.Data.Models;
using MockDoor.Data.Models.Headers;
using MockDoor.Shared.Models.Headers;
using MockDoor.Shared.Models.QueryParameters;
using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.ServiceRequest;

namespace MockDoor.Data.Mappers
{
    public static class RequestMappers
    {
        public static ServiceRequest ToEntity(this ServiceRequestDto serviceRequestDto, bool createNew, bool createChecksumOnResponses)
        {
            return serviceRequestDto == null ? null : new ServiceRequest()
            {
                FromBody = serviceRequestDto.FromBody,
                FromUrl = serviceRequestDto.FromUrl,
                ExactUrlMatch = serviceRequestDto.ExactUrlMatch,
                ExpectAuthHeader = serviceRequestDto.ExpectAuthHeader,
                MockBehaviour = serviceRequestDto.MockBehaviour,
                Enabled = serviceRequestDto.Enabled,
                MicroserviceID = serviceRequestDto.MicroserviceId,
                CreatedUtc = !createNew ? serviceRequestDto.CreatedUtc : DateTime.Now,
                RestType = serviceRequestDto.RestType,
                SimulateTime = serviceRequestDto.SimulateTime,
                TTL = serviceRequestDto.Ttl,
                MockResponses = serviceRequestDto.MockResponses?.ToEntities(createChecksumOnResponses),
                QueryParameters = serviceRequestDto.QueryParameters?.ToEntities(),
                RequestHeaders = serviceRequestDto.RequestHeaders?.ToEntities()
            };
        }

        public static List<ServiceRequest> ToEntities(this List<ServiceRequestDto> serviceRequestDtos, bool createNew, bool createChecksumOnResponses)
        {
            return serviceRequestDtos?.Select(serviceRequestDto => serviceRequestDto.ToEntity(createNew, createChecksumOnResponses)).ToList();
        }

        public static ServiceRequest ToEntity(this UpdateServiceRequestDto serviceRequestDto, bool createChecksumOnResponses)
        {
            return serviceRequestDto == null ? null : new ServiceRequest()
            {
                FromBody = serviceRequestDto.FromBody,
                FromUrl = serviceRequestDto.FromUrl,
                ExactUrlMatch = serviceRequestDto.ExactUrlMatch,
                ExpectAuthHeader = serviceRequestDto.ExpectAuthHeader,
                MockBehaviour = serviceRequestDto.MockBehaviour,
                Enabled = serviceRequestDto.Enabled,
                RestType = serviceRequestDto.RestType,
                SimulateTime = serviceRequestDto.SimulateTime,
                TTL = serviceRequestDto.Ttl,
                MockResponses = serviceRequestDto.Responses?.ToEntities(createChecksumOnResponses),
                QueryParameters = serviceRequestDto.QueryParameters?.ToEntities(),
                RequestHeaders = serviceRequestDto.RequestHeaders?.ToEntities(),
                CreatedUtc = serviceRequestDto.CreatedUtc ?? DateTime.UtcNow
        };
        }

        public static List<ServiceRequest> ToEntities(this List<UpdateServiceRequestDto> serviceRequestDtos, bool createChecksumOnResponses)
        {
            return serviceRequestDtos?.Select(serviceRequestDto => serviceRequestDto.ToEntity(createChecksumOnResponses)).ToList();
        }

        public static ServiceRequestDto ToDto(this ServiceRequest serviceRequest, bool createNew, bool createChecksumOnResponses)
        {
            return serviceRequest == null ? null : new ServiceRequestDto()
            {
                Id = serviceRequest.ID,
                FromBody = serviceRequest.FromBody,
                FromUrl = serviceRequest.FromUrl,
                ExactUrlMatch = serviceRequest.ExactUrlMatch,
                ExpectAuthHeader = serviceRequest.ExpectAuthHeader,
                MockBehaviour = serviceRequest.MockBehaviour,
                Enabled = serviceRequest.Enabled,
                MicroserviceId = serviceRequest.MicroserviceID,
                CreatedUtc = !createNew ? serviceRequest.CreatedUtc : DateTime.Now,
                RestType = serviceRequest.RestType,
                SimulateTime = serviceRequest.SimulateTime,
                Ttl = serviceRequest.TTL,
                MockResponses = serviceRequest.MockResponses?.ToDtos(createChecksumOnResponses),
                QueryParameters = serviceRequest.QueryParameters?.ToDtos(),
                RequestHeaders = serviceRequest.RequestHeaders?.ToDtos()
            };
        }


        public static List<ServiceRequestDto> ToDtos(this List<ServiceRequest> serviceRequests, bool createNew, bool createChecksumOnResponses)
        {
            return serviceRequests?.Select(serviceRequest => serviceRequest.ToDto(createNew, createChecksumOnResponses)).ToList();
        }

        public static UpdateServiceRequestDto ToUpdateDto(this ServiceRequest serviceRequest, bool createChecksumOnResponses)
        {
            return serviceRequest == null ? null : new UpdateServiceRequestDto()
            {
                FromBody = serviceRequest.FromBody,
                FromUrl = serviceRequest.FromUrl,
                ExactUrlMatch = serviceRequest.ExactUrlMatch,
                ExpectAuthHeader = serviceRequest.ExpectAuthHeader,
                MockBehaviour = serviceRequest.MockBehaviour,
                Enabled = serviceRequest.Enabled,
                RestType = serviceRequest.RestType,
                SimulateTime = serviceRequest.SimulateTime,
                Ttl = serviceRequest.TTL,
                Responses = serviceRequest.MockResponses?.ToDtos(createChecksumOnResponses),
                QueryParameters = serviceRequest.QueryParameters?.ToDtos(),
                RequestHeaders = serviceRequest.RequestHeaders?.ToDtos(),
                CreatedUtc = serviceRequest.CreatedUtc
            };
        }

        public static List<UpdateServiceRequestDto> ToUpdateDtos(this List<ServiceRequest> serviceRequests, bool createChecksumOnResponses)
        {
            return serviceRequests?.Select(serviceRequest => serviceRequest.ToUpdateDto(createChecksumOnResponses)).ToList();
        }

        public static UpdateServiceRequestDto ToUpdateDto(this ServiceRequestDto serviceRequest)
        {
            return serviceRequest == null ? null : new UpdateServiceRequestDto()
            {
                FromBody = serviceRequest.FromBody,
                FromUrl = serviceRequest.FromUrl,
                ExactUrlMatch = serviceRequest.ExactUrlMatch,
                ExpectAuthHeader = serviceRequest.ExpectAuthHeader,
                MockBehaviour = serviceRequest.MockBehaviour,
                Enabled = serviceRequest.Enabled,
                RestType = serviceRequest.RestType,
                SimulateTime = serviceRequest.SimulateTime,
                Ttl = serviceRequest.Ttl,
                Responses = serviceRequest.MockResponses,
                QueryParameters = serviceRequest.QueryParameters,
                RequestHeaders = serviceRequest.RequestHeaders,
                CreatedUtc = serviceRequest.CreatedUtc
            };
        }

        public static List<UpdateServiceRequestDto> ToUpdateDtos(this List<ServiceRequestDto> serviceRequests)
        {
            return serviceRequests?.Select(serviceRequest => serviceRequest.ToUpdateDto()).ToList();
        }

        public static ServiceRequest UpdateWithDto(this ServiceRequest baseServiceRequest, UpdateServiceRequestDto serviceRequestDto)
        {
            if (baseServiceRequest == null)
                return null;

            baseServiceRequest.FromBody = serviceRequestDto.FromBody;
            baseServiceRequest.FromUrl = serviceRequestDto.FromUrl;
            baseServiceRequest.ExactUrlMatch = serviceRequestDto.ExactUrlMatch;
            baseServiceRequest.ExpectAuthHeader = serviceRequestDto.ExpectAuthHeader;
            baseServiceRequest.MockBehaviour = serviceRequestDto.MockBehaviour;
            baseServiceRequest.Enabled = serviceRequestDto.Enabled;
            baseServiceRequest.RestType = serviceRequestDto.RestType;
            baseServiceRequest.SimulateTime = serviceRequestDto.SimulateTime;
            baseServiceRequest.TTL = serviceRequestDto.Ttl;
            baseServiceRequest.CreatedUtc = serviceRequestDto.CreatedUtc ?? DateTime.UtcNow;

            MergeResponses(baseServiceRequest, serviceRequestDto.Responses);
            MergeQueryParameters(baseServiceRequest, serviceRequestDto.QueryParameters);
            MergeRequestHeaders(baseServiceRequest, serviceRequestDto.RequestHeaders);
            
            return baseServiceRequest;
        }

        public static ServiceRequest MergeResponses(this ServiceRequest baseServiceRequest, List<MockResponseDto> responses)
        {
            var responsesToAdd = responses.Where(sr => (baseServiceRequest.MockResponses?.All(rr => sr.Id != rr.ID) ?? false)|| sr.Id == 0).ToList();
            var responsesToUpdate = baseServiceRequest.MockResponses.Where(rr => responses.Any(sr => sr.Id == rr.ID && sr.Id > 0));

            baseServiceRequest.MockResponses.RemoveAll(rr => !responses.Any(sr => sr.Id == rr.ID && sr.Id > 0));

            if (responsesToAdd.Any())
            {
                baseServiceRequest.MockResponses.AddRange(responsesToAdd.ToEntities(true));
            }

            foreach(var baseResponse in responsesToUpdate)
            {
                var updatedResponse = responses.First(r => r.Id == baseResponse.ID);
                baseResponse.Body = updatedResponse.Body;
                baseResponse.Encoding = updatedResponse.Encoding;
                baseResponse.Code = updatedResponse.Code;
                baseResponse.Checksum = ChecksumHelpers.CreateDefaultChecksum(updatedResponse);
                baseResponse.CreatedUtc = updatedResponse.CreatedUtc;

                MergeResponseHeaders(baseResponse, updatedResponse.Headers);
            }
            
            
            return baseServiceRequest;
        }

        public static void MergeQueryParameters(this ServiceRequest baseServiceRequest, List<QueryParameterDto> queryParameters)
        {
            var querysToAdd = queryParameters.Where(sr => (baseServiceRequest.QueryParameters?.All(qp => sr.Id != qp.Id) ?? false)|| sr.Id == 0).ToList();
            var querysToUpdate = baseServiceRequest.QueryParameters.Where(qp => queryParameters.Any(sr => sr.Id == qp.Id && sr.Id > 0));

            baseServiceRequest.QueryParameters.RemoveAll(qp => !queryParameters.Any(sr => sr.Id == qp.Id && sr.Id > 0));

            if (querysToAdd.Any())
            {
                baseServiceRequest.QueryParameters.AddRange(querysToAdd.ToEntities());
            }

            foreach(var baseQueryParameter in querysToUpdate)
            {
                var updatedQueries = queryParameters.First(qp => qp.Id == baseQueryParameter.Id);
                baseQueryParameter.Name = updatedQueries.Name;
                baseQueryParameter.Value = updatedQueries.Value;
            }
        }
        
        public static void MergeRequestHeaders(this ServiceRequest baseServiceRequest, List<ServiceRequestHeaderDto> headers)
        {
            if (baseServiceRequest.RequestHeaders == null)
                baseServiceRequest.RequestHeaders = new List<RequestHeader>();

            if (headers == null)
                headers = new List<ServiceRequestHeaderDto>();
            
            var headersToAdd = headers.Where(h => baseServiceRequest.RequestHeaders.All(rr => h.Id != rr.ID)|| h.Id == 0).ToList();
            var headersToUpdate = baseServiceRequest.RequestHeaders.Where(rr => headers.Any(h => h.Id == rr.ID && h.Id > 0));

            baseServiceRequest.RequestHeaders.RemoveAll(rr => !headers.Any(sr => sr.Id == rr.ID && sr.Id > 0));

            if (headersToAdd.Any())
            {
                baseServiceRequest.RequestHeaders.AddRange(headersToAdd.ToEntities());
            }

            foreach(var baseHeader in headersToUpdate)
            {
                var updatedHeader = headers.First(h => h.Id == baseHeader.ID);
                baseHeader.Name = updatedHeader.Name;
                baseHeader.Value = string.Join(';', updatedHeader.Value);
            }
        }
        
        public static void MergeResponseHeaders(this MockResponse baseMockResponse, List<MockResponseHeaderDto> headers)
        {
            if (baseMockResponse.Headers == null)
                baseMockResponse.Headers = new List<ResponseHeader>();

            if (headers == null)
                headers = new List<MockResponseHeaderDto>();
            
            var headersToAdd = headers.Where(h => baseMockResponse.Headers.All(rr => h.Id != rr.ID)|| h.Id == 0).ToList();
            var headersToUpdate = baseMockResponse.Headers.Where(rr => headers.Any(h => h.Id == rr.ID && h.Id > 0));

            baseMockResponse.Headers.RemoveAll(rr => !headers.Any(sr => sr.Id == rr.ID && sr.Id > 0));

            if (headersToAdd.Any())
            {
                baseMockResponse.Headers.AddRange(headersToAdd.ToEntities());
            }

            foreach(var baseHeader in headersToUpdate)
            {
                var updatedHeader = headers.First(h => h.Id == baseHeader.ID);
                baseHeader.Name = updatedHeader.Name;
                baseHeader.Value = string.Join(';', updatedHeader.Value);
            }
        }
    }
}
