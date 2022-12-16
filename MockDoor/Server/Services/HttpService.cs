using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MockDoor.Abstractions.MockServices;
using MockDoor.Abstractions.ProxyServices;
using MockDoor.Services.Helpers;
using MockDoor.Services.Hubs;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Configuration;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.General;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.ServiceRequest;

namespace MockDoor.Server.Services
{
    public class HttpService : IHttpService
    {
        private readonly IMockService _mockService;
        private readonly IProxyService _proxyService;
        private readonly DeploymentConfiguration _deploymentConfiguration;
        private readonly IHubContext<RequestHub> _hubcontext;

        public HttpService(IMockService mockService, IProxyService proxyService, IOptions<DeploymentConfiguration> deploymentConfigurationOptions, IHubContext<RequestHub> hubcontext)
        {
            _mockService = mockService ?? throw new ArgumentNullException(nameof(mockService));
            _proxyService = proxyService ?? throw new ArgumentNullException(nameof(proxyService));
            _deploymentConfiguration = deploymentConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(deploymentConfigurationOptions));
            _hubcontext = hubcontext ?? throw new ArgumentNullException(nameof(hubcontext));
        }

        public async Task<IActionResult> ProcessMicroserviceRequestAsync(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, RestType restType, HttpContext context, string endpointPath)
        {
            if (context == null)
                throw new ArgumentNullException($"Error {nameof(context)} is null");
                
            await SendLiveFeedMessageAsync(context, endpointPath, matchingRequestMicroserviceDetails.Microservice.Id);
            
            var foundRequest = await _mockService.GetMatchingServiceRequestDtoAsync(matchingRequestMicroserviceDetails, restType, context, endpointPath);
            
            // If mock mode or simulation time applied
            if (!matchingRequestMicroserviceDetails.Microservice.ProxyMode || HasSimulationApplied(matchingRequestMicroserviceDetails, foundRequest))
            {
                // To handle rereading the content better this is handled in the proxy service separately
                // Here we send the message with a fresh httpclient as it will only be sent once
                await SendDebuggerMessageAsync(restType, context, endpointPath);
                if (foundRequest != null)
                {
                    //check auth header
                    if (foundRequest.ExpectAuthHeader && !IsAuthHeaderPresent(context.Request.Headers))
                    {
                        return new UnauthorizedResult();
                    }
                    
                    var existingResponse = await _mockService.GetMockResponseAsync(matchingRequestMicroserviceDetails, restType, context, endpointPath);

                    if(existingResponse != null)
                    {
                        var headersToAdd = HttpHelpers.GetResponseHeadersToAdd(matchingRequestMicroserviceDetails.Microservice,
                                                                                             existingResponse.Headers
                                                                                             .Where(h => h.Name.ToLower() != "host" && 
                                                                                                                             h.Name.ToLower() != "transfer-encoding")
                                                                                             .Select(h => new HeaderItem(h.Name, string.Join(';', h.Value))));

                        foreach (var headerItem in headersToAdd)
                        {
                            context.Response.Headers.TryAdd(headerItem.Name, string.Join(";", headerItem.Value));
                        }
                        
                        return new ContentResult()
                        {
                            Content = existingResponse.Body,
                            ContentType = existingResponse.ContentType,
                            StatusCode = (int)existingResponse.Code
                        };
                    }
                }
                return null;
            }
            else
            {
                var response = await _proxyService.ProxyRequestToMicroserviceAsync(matchingRequestMicroserviceDetails, restType, context, endpointPath);

                return response;
            }
        }

        private async Task SendDebuggerMessageAsync(RestType restType, HttpContext context, string endpointPath)
        {
            if ((_deploymentConfiguration?.Debug ?? false) && !string.IsNullOrWhiteSpace(_deploymentConfiguration.DebuggerUrl))
            {
                using var client = new HttpClient();
                var requestBody = await GeneralHelpers.RequestBodyToStringAsync(context?.Request);
                try
                {
                    var httpRequestDebuggerLog = new HttpRequestMessage();
                    httpRequestDebuggerLog.RequestUri = new Uri(_deploymentConfiguration.DebuggerUrl + endpointPath);
                    httpRequestDebuggerLog.Content =
                        ConvertHelper.ToExactStringContent(requestBody, context?.Request.ContentType);
                    httpRequestDebuggerLog.Headers.Clear();

                    switch (restType)
                    {
                        case RestType.GET:
                            httpRequestDebuggerLog.Method = HttpMethod.Get;
                            break;
                        case RestType.DELETE:
                            httpRequestDebuggerLog.Method = HttpMethod.Delete;
                            break;
                        case RestType.POST:
                            httpRequestDebuggerLog.Method = HttpMethod.Post;
                            break;
                        case RestType.PUT:
                            httpRequestDebuggerLog.Method = HttpMethod.Put;
                            break;
                        case RestType.PATCH:
                            httpRequestDebuggerLog.Method = HttpMethod.Patch;
                            break;
                    }

                    if (context?.Request.Headers != null)
                        foreach (var header in context.Request.Headers)
                        {
                            httpRequestDebuggerLog.Headers.Add(header.Key, (IEnumerable<string>)header.Value);
                        }

                    await client.SendAsync(httpRequestDebuggerLog);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
        
        private async Task SendLiveFeedMessageAsync(HttpContext context, string endpointPath, int microserviceId)
        {
            var request = new HttpRequestDto()
            {
                Timestamp = DateTime.Now,
                HttpMethod = context.Request.Method,
                Endpoint = endpointPath,
                QueryString = context.Request.QueryString.ToString(),
                Body = await GeneralHelpers.RequestBodyToStringAsync(context.Request),
                Headers = context.Request.Headers.ToDictionary(a => a.Key, a => string.Join(";", a.Value))
            };

            await _hubcontext.Clients.All.SendAsync($"{microserviceId}/SendRequest", request);
        }

        private static bool IsAuthHeaderPresent(IHeaderDictionary headers)
        {
            return headers?.Any(h => h.Key.ToLower().Equals("authorization")) ?? false;
        }

        private bool HasSimulationApplied(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, ServiceRequestDto serviceRequestDto)
        {
            return matchingRequestMicroserviceDetails.Microservice.SimulateTime != null || matchingRequestMicroserviceDetails.ServiceGroup.SimulateTime != null || matchingRequestMicroserviceDetails.Tenant.SimulateTime != null || serviceRequestDto?.SimulateTime != null;
        }
    }
}
