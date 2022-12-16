using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using MockDoor.Abstractions.MockServices;
using MockDoor.Abstractions.Repositories;
using MockDoor.Data.Helpers;
using MockDoor.Services.Helpers;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Headers;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.QueryParameters;
using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.ServiceRequest;

namespace MockDoor.Services.MockServices
{
    public class MockService : IMockService
    {
        private static readonly Random RandomNumberGenerator = new();

        private readonly IServiceRequestRepository _serviceRequestRepository;

        public MockService(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<ServiceRequestDto> GetMatchingServiceRequestDtoAsync(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, RestType restType, HttpContext context, string endpointPath)
        {
            string body = restType != RestType.GET ? await GeneralHelpers.RequestBodyToStringAsync(context?.Request) : null;

            return await FindMatchingServiceRequestAsync(matchingRequestMicroserviceDetails.Microservice.Id, context, restType, endpointPath, body);
        }

        public async Task<MockResponseDto> GetMockResponseAsync(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, RestType restType, HttpContext context, string endpointPath)
        {
            DateTime? resolvedSimulateTime = matchingRequestMicroserviceDetails.Microservice.SimulateTime?.AddSeconds(1) ??
                                    matchingRequestMicroserviceDetails.ServiceGroup.SimulateTime?.AddSeconds(1) ??
                                    matchingRequestMicroserviceDetails.Tenant.SimulateTime?.AddSeconds(1);

            var matchingRequestDto = await GetMatchingServiceRequestDtoAsync(matchingRequestMicroserviceDetails, restType, context, endpointPath);

            MockResponseDto existingResponse = InnerGetMockResponse(matchingRequestDto, matchingRequestMicroserviceDetails.Microservice.RandomiseMockResult, resolvedSimulateTime);

            await Task.Delay(matchingRequestMicroserviceDetails.Microservice.FakeDelay);

            return existingResponse;
        }

        public async Task CreateMockResponseIfNotExistAsync(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, HttpContext context, RestType restType, string endpointPath, string requestBody, HttpResponseMessage response, TimeSpan latency)
        {
            var request = await FindMatchingServiceRequestAsync(matchingRequestMicroserviceDetails.Microservice.Id, context, restType, endpointPath, requestBody);

            if (request == null || request.MockBehaviour == MockBehaviour.AutoMockWithProxy)
            {
                var mockResponse = await BuildMockResponseAsync(matchingRequestMicroserviceDetails, response, latency);

                if (request == null)
                {
                    var serviceRequest = BuildMockServiceRequestUsingResponse(matchingRequestMicroserviceDetails,
                        context, restType, endpointPath, requestBody, mockResponse);

                    await _serviceRequestRepository.CreateServiceRequestAsync(
                        matchingRequestMicroserviceDetails.Microservice.Id, serviceRequest);
                }
                else if (!DoesResponseExist(request, mockResponse))
                {
                    // no responses found add to requests list of responses
                    await _serviceRequestRepository.AddResponseToRequestAsync(request.Id, mockResponse);
                }
            }
        }

        private static async Task<MockResponseDto> BuildMockResponseAsync(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, HttpResponseMessage response, TimeSpan latency)
        {
            if (response == null)
            {
                return null;
            }
            
            var responseContent = await response.Content.ReadAsStringAsync()!;
            var responseContentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";

            var mockResponse = new MockResponseDto
            {
                Body = responseContent,
                ContentType = responseContentType,
                Encoding = SupportedEncodingType.UTF8,
                Latency = latency,
                Checksum = ChecksumHelpers.CreateChecksum(SupportedEncodingType.UTF8, $"{responseContent}-{response.StatusCode}-{responseContentType}"),
                Code = response.StatusCode
            };

            mockResponse.Headers = GetResponseHeaders(matchingRequestMicroserviceDetails, response);

            return mockResponse;
        }

        private static ServiceRequestDto BuildMockServiceRequestUsingResponse(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, HttpContext context, RestType restType, string endpointPath, string requestBody, MockResponseDto newResponse)
        {
            var queryParams = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());

            //no request so create new request for the provided response
            var serviceRequest = new ServiceRequestDto()
            {
                MicroserviceId = matchingRequestMicroserviceDetails.Microservice.Id,
                ExactUrlMatch = true,
                FromBody = requestBody,
                FromUrl = endpointPath,
                RestType = restType,
                MockResponses = new List<MockResponseDto>() { newResponse },
                Enabled = true,
                RequestHeaders = GetRequestHeaders(matchingRequestMicroserviceDetails, context),
                ExpectAuthHeader = context.Request.Headers.Any(h => h.Key.ToLower() == "authorization"),
                QueryParameters = queryParams.AllKeys.Select((k, i) => new QueryParameterDto() { Name = k, Value = queryParams[k], OrderIndex = i}).ToList()
            };
            
            if (matchingRequestMicroserviceDetails.Microservice.Headers != null && matchingRequestMicroserviceDetails.Microservice.Headers.Count > 0)
            {
                serviceRequest.ExpectAuthHeader = serviceRequest.RequestHeaders?.Any(h => h.Name.ToLower() == "authorization") ?? false;
            }

            return serviceRequest;
        }
        
        private static List<ServiceRequestHeaderDto> GetRequestHeaders(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, HttpContext context)
        {
            var serviceHeaders = new List<ServiceRequestHeaderDto>();
            
            foreach (var header in context.Request.Headers.Where(h => h.Key.ToLower() != "host"))
            {
                if (header.Value != default(StringValues))
                {
                    switch (matchingRequestMicroserviceDetails.Microservice.HeadersMode)
                    {
                        case HeadersMode.All:
                        {
                            serviceHeaders.Add(new ServiceRequestHeaderDto() { Name = header.Key, Value = header.Value });
                        } break;
                        case HeadersMode.UserDefined:
                        {
                            var matchingHeader = matchingRequestMicroserviceDetails.Microservice.Headers?.Any(h => h.Enabled && h.Outgoing &&  h.Name.ToLower() == header.Key.ToLower());
                            if (matchingHeader ?? false)
                            {
                                serviceHeaders.Add(new ServiceRequestHeaderDto() { Name = header.Key, Value = header.Value });
                            }
                        } break;
                    }
                }
            }
            return serviceHeaders;
        }

        private static List<MockResponseHeaderDto> GetResponseHeaders(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, HttpResponseMessage response)
        {
            var responseHeaders = new List<MockResponseHeaderDto>();
            foreach (var header in response.Headers.Where(h => h.Key.ToLower() != "host"))
            {
                if (header.Value != default(StringValues))
                {
                    switch (matchingRequestMicroserviceDetails.Microservice.HeadersMode)
                    {
                        case HeadersMode.All:
                        {
                            responseHeaders.Add(new MockResponseHeaderDto() { Name = header.Key, Value = string.Join(';', header.Value) });
                        } break;
                        case HeadersMode.UserDefined:
                        {
                            var matchingHeader = matchingRequestMicroserviceDetails.Microservice.Headers?.Any(h => h.Enabled && h.Incoming &&  h.Name.ToLower() == header.Key.ToLower());
                            if (matchingHeader ?? false)
                            {
                                responseHeaders.Add(new MockResponseHeaderDto() { Name = header.Key, Value = string.Join(';', header.Value) });
                            }
                        } break;
                    }
                }
            }

            return responseHeaders;
        }

        public async Task<ServiceRequestDto> FindMatchingServiceRequestAsync(int microserviceId, HttpContext context, RestType restType, string endpoint, string requestBody)
        {
            var allMicroserviceRequests = await _serviceRequestRepository.GetAllServiceRequestsForMicroserviceAsync(microserviceId);

            foreach (var serviceRequest in allMicroserviceRequests.Where(mc => mc.RestType == restType))
            {
                if (CompareRequest($"{endpoint}", requestBody, serviceRequest, context.Request.QueryString))
                {
                    return serviceRequest;
                }
            }

            Console.WriteLine($"No request found for\tEnd point: {endpoint}\tBody: {requestBody}");
            return null;
        }

        private MockResponseDto InnerGetMockResponse(ServiceRequestDto request, bool pickRandom, DateTime? simulateTime)
        {
            if (request != null && request.MockBehaviour != MockBehaviour.ProxyOnly)
            {
                var resolvedSimulateTime = request.SimulateTime?.AddSeconds(1) ?? simulateTime?.AddSeconds(1);

                Console.WriteLine($"Request match found: {request.Id}");

                var enabledResponses = request.MockResponses.Where(r => r.Enabled)
                                                                               .Where(er => resolvedSimulateTime == null || er.CreatedUtc < resolvedSimulateTime)
                                                                               .ToList();

                var filteredAndOrderedResponses = enabledResponses.OrderBy(er => er.Priority).ThenByDescending(er => er.CreatedUtc).ToList();

                if (filteredAndOrderedResponses.Count > 0)
                {
                    if (!pickRandom)
                    {
                        return filteredAndOrderedResponses.First();
                    }

                    return filteredAndOrderedResponses[RandomNumberGenerator.Next(enabledResponses.Count())];
                }
            }

            return null;
        }

        private static bool CompareRequest(string endpoint, string requestBody, ServiceRequestDto serviceRequest, QueryString queryString)
        {
                bool bodyMatches = requestBody == null || requestBody.ToLower().Equals(serviceRequest.FromBody?.ToLower()) || (string.IsNullOrEmpty(requestBody) && string.IsNullOrEmpty(serviceRequest.FromBody));

                bool queryParametersMatch = false;

                if (!queryString.HasValue && serviceRequest.QueryParameters?.Count == 0)
                {
                    queryParametersMatch = true;
                }
                else if (queryString.HasValue && serviceRequest.QueryParameters?.Count > 0)
                {
                    queryParametersMatch = CompareQueryParameters(queryString, serviceRequest.QueryParameters);
                }

                // Exact match check
                if (serviceRequest.ExactUrlMatch &&
                    serviceRequest.FromUrl.ToLower().Equals(endpoint.ToLower()) &&
                    bodyMatches)
                {
                    return queryParametersMatch;
                }

                // starts with check
                if (!serviceRequest.ExactUrlMatch && endpoint.ToLower().StartsWith(serviceRequest.FromUrl.ToLower()) && bodyMatches)
                {
                    return queryParametersMatch;
                }

                return false;
        }

        private static bool CompareQueryParameters(QueryString queryString, List<QueryParameterDto> queryParameters)
        {
            var requestQueryParameters = queryString.Value.TrimStart('?').Split("&").ToHashSet();

            if (requestQueryParameters.Count != queryParameters.Count)
            {
                return false;
            }
            
            foreach (var queryParameter in queryParameters)
            {
                if(!requestQueryParameters.Contains($"{queryParameter.Name}={queryParameter.Value}"))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool DoesResponseExist(ServiceRequestDto request, MockResponseDto newResponse)
        {
            return request.MockResponses.Any(rr => newResponse.Checksum.Equals(rr.Checksum));
        }
    }
}
