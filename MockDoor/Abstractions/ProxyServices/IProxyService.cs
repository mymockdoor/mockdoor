using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Microservice;

namespace MockDoor.Abstractions.ProxyServices
{
    public interface IProxyService
    {
        Task<IActionResult> ProxyRequestToMicroserviceAsync(MatchingRequestMicroserviceDetailsDto microservice, RestType restType, HttpContext context, string endpointPath);
    }
}