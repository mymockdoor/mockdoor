﻿using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MockDoor.Abstractions.MockServices;
using MockDoor.Abstractions.ProxyServices;
using MockDoor.Services.Helpers;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Configuration;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.General;
using MockDoor.Shared.Models.Microservice;
using Newtonsoft.Json;

namespace MockDoor.Services.ProxyServices
{
    public class ProxyService : IProxyService
    {
        readonly IMockService _mockService;
        private readonly DeploymentConfiguration _deploymentConfiguration;

        public ProxyService(IMockService mockService, IOptions<DeploymentConfiguration> deploymentConfigurationOptions)
        {
            _mockService = mockService ?? throw new ArgumentNullException(nameof(mockService));
            _deploymentConfiguration = deploymentConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(deploymentConfigurationOptions));
        }

        public async Task<IActionResult> ProxyRequestToMicroserviceAsync(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails, RestType restType, HttpContext context, string endpointPath)
        {
            if (!string.IsNullOrWhiteSpace(matchingRequestMicroserviceDetails.Microservice.TargetUrl))
            {
                string queryString = context.Request.QueryString.ToString();

                string requestBody = null, contentType = null;

                if (restType != RestType.GET && restType != RestType.DELETE)
                {
                    requestBody = await GeneralHelpers.RequestBodyToStringAsync(context.Request);
                    contentType = context.Request?.ContentType;
                }

                var resolvedEndpoint = matchingRequestMicroserviceDetails.Microservice.PassThroughTenant ? $"{matchingRequestMicroserviceDetails.Tenant.Path}/{endpointPath}" : endpointPath;

                var matchingRequest = await _mockService.FindMatchingServiceRequestAsync(matchingRequestMicroserviceDetails.Microservice.Id, context,
                                                                        restType, $"{resolvedEndpoint}{queryString}", requestBody );

                if (matchingRequest != null && matchingRequest.MockBehaviour == MockBehaviour.MockOnly)
                {
                    return new NotFoundResult();
                }
                
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var response = await SendRequestAsync(matchingRequestMicroserviceDetails.Microservice, restType, context, requestBody, contentType, $"{resolvedEndpoint}{queryString}");
                stopWatch.Stop();

                await _mockService.CreateMockResponseIfNotExistAsync(matchingRequestMicroserviceDetails, context, restType, endpointPath, requestBody, response, stopWatch.Elapsed);

                
                if (response != null)
                {                
                    if (matchingRequest is { MockBehaviour: MockBehaviour.ProxyOnly } && 
                        response.Content.Headers.ContentType?.MediaType != null &&
                        response.Content.Headers.ContentType.MediaType.ToUpper().Contains("IMAGE"))
                    {
                        var contentReturnedBytes = await response.Content.ReadAsByteArrayAsync();
                        return new FileContentResult(contentReturnedBytes, response.Content.Headers.ContentType.MediaType);
                    }
                
                    var contentReturned = await response.Content.ReadAsStringAsync();

                     return new ContentResult()
                    {
                        Content = contentReturned,
                        ContentType = response.Content.Headers.ContentType?.MediaType ?? "application/json",
                        StatusCode = Convert.ToInt32(response.StatusCode)
                    };
                }
                else
                {
                    return new BadRequestObjectResult("Unexpected error occured getting the response");
                }
            }
            return new BadRequestObjectResult("Mock Microservice in Proxy mode but no target url is set");
        }

        private async Task<HttpResponseMessage> SendRequestAsync(MicroserviceResultDto microservice, RestType restType, HttpContext context, string requestBody, string contentType, string endpointPath)
        {
            if (microservice != null)
            {
                var httpRequestMessage = new HttpRequestMessage();
                try
                {
                    Console.WriteLine("Headers Mode: " + microservice.HeadersMode);
                    Console.WriteLine("Microservice Headers: " + JsonConvert.SerializeObject(microservice.Headers));
                    
                    SetRequestHeaders(microservice, context, httpRequestMessage);

                    switch (restType)
                    {
                        case RestType.GET: httpRequestMessage.Method = HttpMethod.Get; break;
                        case RestType.DELETE: httpRequestMessage.Method = HttpMethod.Delete; break;
                        case RestType.POST: httpRequestMessage.Method = HttpMethod.Post; break;
                        case RestType.PUT: httpRequestMessage.Method = HttpMethod.Put; break;
                        case RestType.PATCH: httpRequestMessage.Method = HttpMethod.Patch; break;
                    }

                    if (restType != RestType.GET && restType != RestType.DELETE)
                    {
                        httpRequestMessage.Content = ConvertHelper.ToExactStringContent(requestBody, contentType);
                    }
                    
                    using var client = new HttpClient();
                    httpRequestMessage.RequestUri = new Uri(microservice.TargetUrl + endpointPath);
                    
                    var response = await client.SendAsync(httpRequestMessage);


                    var headersToAdd = HttpHelpers.GetResponseHeadersToAdd(microservice,
                            context.Request.Headers.Where(h => h.Key.ToLower() != "host" &&
                                                               h.Key.ToLower() != "transfer-encoding")
                            .Select(h => new HeaderItem(h.Key, h.Value)));

                    foreach (var headerItem in headersToAdd)
                    {
                        try
                        {
                            context.Response.Headers.TryAdd(headerItem.Name, string.Join(";", headerItem.Value));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    if ((_deploymentConfiguration?.Debug ?? false) && !string.IsNullOrWhiteSpace(_deploymentConfiguration.DebuggerUrl))
                    {
                        try
                        {
                            var httpRequestDebuggerLog = new HttpRequestMessage();
                            httpRequestDebuggerLog.RequestUri = new Uri(_deploymentConfiguration.DebuggerUrl + endpointPath);
                            httpRequestDebuggerLog.Method = httpRequestMessage.Method;
                            httpRequestDebuggerLog.Content = httpRequestMessage.Content;
                            httpRequestDebuggerLog.Headers.Clear();

                            foreach (var header in httpRequestMessage.Headers)
                            {
                                httpRequestDebuggerLog.Headers.Add(header.Key, header.Value);
                            }
                            await client.SendAsync(httpRequestDebuggerLog);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                    
                    return response;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            throw new ArgumentException("Invalid Microservice");
        }

        private static void SetRequestHeaders(MicroserviceResultDto microservice, HttpContext context,
            HttpRequestMessage httpRequestMessage)
        {
            foreach (var header in context.Request.Headers.Where(h => h.Key.ToLower() != "host"))
            {
                if (header.Value != default(StringValues))
                {
                    switch (microservice.HeadersMode)
                    {
                        case HeadersMode.All:
                        {
                            httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);
                        }
                            break;
                        case HeadersMode.UserDefined:
                        {
                            var matchingHeader = microservice.Headers?.Any(h =>
                                h.Enabled && h.Outgoing && h.Name.ToUpper().Equals(header.Key.ToUpper()));
                            if (matchingHeader ?? false)
                            {
                                httpRequestMessage.Headers.TryAddWithoutValidation(header.Key,
                                    (IEnumerable<string>)header.Value);
                            }
                        } break;
                    }
                }
            }
        }
    }
}
