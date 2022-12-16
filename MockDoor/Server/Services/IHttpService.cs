using Microsoft.AspNetCore.Mvc;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Microservice;

namespace MockDoor.Server.Services
{
    public interface IHttpService
    {
        Task<IActionResult> ProcessMicroserviceRequestAsync(MatchingRequestMicroserviceDetailsDto microservice, RestType restType, HttpContext context, string endpointPath);
    }
}