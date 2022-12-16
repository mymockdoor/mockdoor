using Microsoft.AspNetCore.Mvc;
using MockDoor.Abstractions.Repositories;
using MockDoor.Server.Constants;
using MockDoor.Server.Services;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Microservice;

namespace MockDoor.Server.Controllers.MockControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockController : ControllerBase
    {
        private readonly ILogger<MockController> _logger;
        private readonly IMicroserviceRepository _microserviceRepository;
        private readonly IHttpService _httpServices;
         
        public MockController(ILogger<MockController> logger, IMicroserviceRepository microserviceRepository, IHttpService httpServices)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _microserviceRepository = microserviceRepository ?? throw new ArgumentNullException(nameof(microserviceRepository));
            _httpServices = httpServices ?? throw new ArgumentNullException(nameof(httpServices));
        }

        [HttpGet("tgm/{tenantPath}/{groupPath}/{microservicePath}")]
        [HttpGet("mtg/{microservicePath}/{tenantPath}/{groupPath}")]
        [HttpGet("gmt/{groupPath}/{microservicePath}/{tenantPath}")]
        [HttpGet("mgt/{microservicePath}/{groupPath}/{tenantPath}")]
        [HttpGet("tmg/{tenantPath}/{microservicePath}/{groupPath}")]
        [HttpGet("gtm/{groupPath}/{tenantPath}/{microservicePath}")]
        public async Task<IActionResult> CallMicroserviceAsync(string tenantPath, string groupPath, string microservicePath)
        {
            _logger.LogInformation("Call to mock  microservice");
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.GET, HttpContext, string.Empty);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);
            
            if(response == null)
            {
                return NotFound();
            }
            return response;
        }

        [HttpGet("tgm/{tenantPath}/{groupPath}/{microservicePath}/{*endpointPath}")]
        [HttpGet("mtg/{microservicePath}/{tenantPath}/{groupPath}/{*endpointPath}")]
        [HttpGet("gmt/{groupPath}/{microservicePath}/{tenantPath}/{*endpointPath}")]
        [HttpGet("mgt/{microservicePath}/{groupPath}/{tenantPath}/{*endpointPath}")]
        [HttpGet("tmg/{tenantPath}/{microservicePath}/{groupPath}/{*endpointPath}")]
        [HttpGet("gtm/{groupPath}/{tenantPath}/{microservicePath}/{*endpointPath}")]
        public async Task<IActionResult> CallMicroserviceAsync(string tenantPath, string groupPath, string microservicePath, string endpointPath)
        {
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.GET, HttpContext, endpointPath);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);

            if (response == null)
            {
                return NotFound();
            }
            return response;
        }

        [HttpPost("tgm/{tenantPath}/{groupPath}/{microservicePath}")]
        [HttpPost("mtg/{microservicePath}/{tenantPath}/{groupPath}")]
        [HttpPost("gmt/{groupPath}/{microservicePath}/{tenantPath}")]
        [HttpPost("mgt/{microservicePath}/{groupPath}/{tenantPath}")]
        [HttpPost("tmg/{tenantPath}/{microservicePath}/{groupPath}")]
        [HttpPost("gtm/{groupPath}/{tenantPath}/{microservicePath}")]
        public async Task<IActionResult> CallPostMicroserviceAsync(string tenantPath, string groupPath, string microservicePath)
        {
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.POST, HttpContext, string.Empty);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);
            
            if (response == null)
            {
                return NotFound();
            }
            return response;
        }

        [HttpPost("tgm/{tenantPath}/{groupPath}/{microservicePath}/{*endpointPath}")]
        [HttpPost("mtg/{microservicePath}/{tenantPath}/{groupPath}/{*endpointPath}")]
        [HttpPost("gmt/{groupPath}/{microservicePath}/{tenantPath}/{*endpointPath}")]
        [HttpPost("mgt/{microservicePath}/{groupPath}/{tenantPath}/{*endpointPath}")]
        [HttpPost("tmg/{tenantPath}/{microservicePath}/{groupPath}/{*endpointPath}")]
        [HttpPost("gtm/{groupPath}/{tenantPath}/{microservicePath}/{*endpointPath}")]
        public async Task<IActionResult> CallPostMicroserviceAsync(string tenantPath, string groupPath, string microservicePath, string endpointPath)
        {
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.POST, HttpContext, endpointPath);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);
            
            if (response == null)
            {
                return NotFound();
            }
            return response;
        }


        [HttpPut("tgm/{tenantPath}/{groupPath}/{microservicePath}")]
        [HttpPut("mtg/{microservicePath}/{tenantPath}/{groupPath}")]
        [HttpPut("gmt/{groupPath}/{microservicePath}/{tenantPath}")]
        [HttpPut("mgt/{microservicePath}/{groupPath}/{tenantPath}")]
        [HttpPut("tmg/{tenantPath}/{microservicePath}/{groupPath}")]
        [HttpPut("gtm/{groupPath}/{tenantPath}/{microservicePath}")]
        public async Task<IActionResult> CallPutMicroserviceAsync(string tenantPath, string groupPath, string microservicePath)
        {
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.PUT, HttpContext, string.Empty);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);

            if (response == null)
            {
                return NotFound();
            }
            return response;
        }

        [HttpPut("tgm/{tenantPath}/{groupPath}/{microservicePath}/{*endpointPath}")]
        [HttpPut("mtg/{microservicePath}/{tenantPath}/{groupPath}/{*endpointPath}")]
        [HttpPut("gmt/{groupPath}/{microservicePath}/{tenantPath}/{*endpointPath}")]
        [HttpPut("mgt/{microservicePath}/{groupPath}/{tenantPath}/{*endpointPath}")]
        [HttpPut("tmg/{tenantPath}/{microservicePath}/{groupPath}/{*endpointPath}")]
        [HttpPut("gtm/{groupPath}/{tenantPath}/{microservicePath}/{*endpointPath}")]
        public async Task<IActionResult> CallPutMicroserviceAsync(string tenantPath, string groupPath, string microservicePath, string endpointPath)
        {
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.PUT, HttpContext, endpointPath);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);

            if (response == null)
            {
                return NotFound();
            }
            return response;
        }

        [HttpPatch("tgm/{tenantPath}/{groupPath}/{microservicePath}")]
        [HttpPatch("mtg/{microservicePath}/{tenantPath}/{groupPath}")]
        [HttpPatch("gmt/{groupPath}/{microservicePath}/{tenantPath}")]
        [HttpPatch("mgt/{microservicePath}/{groupPath}/{tenantPath}")]
        [HttpPatch("tmg/{tenantPath}/{microservicePath}/{groupPath}")]
        [HttpPatch("gtm/{groupPath}/{tenantPath}/{microservicePath}")]
        public async Task<IActionResult> CallPatchMicroserviceAsync(string tenantPath, string groupPath, string microservicePath)
        {
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.PATCH, HttpContext, string.Empty);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);

            if (response == null)
            {
                return NotFound();
            }
            return response;
        }

        [HttpPatch("tgm/{tenantPath}/{groupPath}/{microservicePath}/{*endpointPath}")]
        [HttpPatch("mtg/{microservicePath}/{tenantPath}/{groupPath}/{*endpointPath}")]
        [HttpPatch("gmt/{groupPath}/{microservicePath}/{tenantPath}/{*endpointPath}")]
        [HttpPatch("mgt/{microservicePath}/{groupPath}/{tenantPath}/{*endpointPath}")]
        [HttpPatch("tmg/{tenantPath}/{microservicePath}/{groupPath}/{*endpointPath}")]
        [HttpPatch("gtm/{groupPath}/{tenantPath}/{microservicePath}/{*endpointPath}")]
        public async Task<IActionResult> CallPatchMicroserviceAsync(string tenantPath, string groupPath, string microservicePath, string endpointPath)
        {
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.PATCH, HttpContext, endpointPath);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);

            if (response == null)
            {
                return NotFound();
            }
            return response;
        }

        [HttpDelete("tgm/{tenantPath}/{groupPath}/{microservicePath}")]
        [HttpDelete("mtg/{microservicePath}/{tenantPath}/{groupPath}")]
        [HttpDelete("gmt/{groupPath}/{microservicePath}/{tenantPath}")]
        [HttpDelete("mgt/{microservicePath}/{groupPath}/{tenantPath}")]
        [HttpDelete("tmg/{tenantPath}/{microservicePath}/{groupPath}")]
        [HttpDelete("gtm/{groupPath}/{tenantPath}/{microservicePath}")]
        public async Task<IActionResult> CallDeleteMicroserviceAsync(string tenantPath, string groupPath, string microservicePath)
        {
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.DELETE, HttpContext, string.Empty);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);

            if (response == null)
            {
                return NotFound();
            }
            return response;
        }

        [HttpDelete("tgm/{tenantPath}/{groupPath}/{microservicePath}/{*endpointPath}")]
        [HttpDelete("mtg/{microservicePath}/{tenantPath}/{groupPath}/{*endpointPath}")]
        [HttpDelete("gmt/{groupPath}/{microservicePath}/{tenantPath}/{*endpointPath}")]
        [HttpDelete("mgt/{microservicePath}/{groupPath}/{tenantPath}/{*endpointPath}")]
        [HttpDelete("tmg/{tenantPath}/{microservicePath}/{groupPath}/{*endpointPath}")]
        [HttpDelete("gtm/{groupPath}/{tenantPath}/{microservicePath}/{*endpointPath}")]
        public async Task<IActionResult> CallDeleteMicroserviceAsync(string tenantPath, string groupPath, string microservicePath, string endpointPath)
        {
            var microservice = await _microserviceRepository.FindMatchingRequest(tenantPath, groupPath, microservicePath);

            var (isValid, result) = ValidateCallMicroservice(microservice);

            if (!isValid)
                return result;

            var response = await _httpServices.ProcessMicroserviceRequestAsync(microservice, RestType.DELETE, HttpContext, endpointPath);

            HttpContext.Response.Headers.Remove(HttpConstants.CustomBasePathHeaderKey);

            if (response == null)
            {
                return NotFound();
            }
            return response;
        }

        /// <summary>
        /// Check Microservice is valid and enabled
        /// </summary>
        /// <param name="matchingRequestMicroserviceDetails"></param>
        /// <returns>return false and bad request if validation fails</returns>
        private (bool isValid, IActionResult result) ValidateCallMicroservice(MatchingRequestMicroserviceDetailsDto matchingRequestMicroserviceDetails)
        {
            if (matchingRequestMicroserviceDetails?.Microservice == null)
                return (false, NotFound(ErrorMessageConstants.MicroserviceNotFound));

            if (matchingRequestMicroserviceDetails.Tenant == null)
                return (false, BadRequest(string.Format(ErrorMessageConstants.MicroserviceAssociatedXFailedToLoad, "tenant")));

            if (matchingRequestMicroserviceDetails.ServiceGroup == null)
                return (false, BadRequest(string.Format(ErrorMessageConstants.MicroserviceAssociatedXFailedToLoad, "service group")));

            if (!matchingRequestMicroserviceDetails.Microservice.Enabled)
                return (false, BadRequest(ErrorMessageConstants.MicroserviceDisabled));

            if (!matchingRequestMicroserviceDetails.ServiceGroup.Enabled)
                return (false, BadRequest(ErrorMessageConstants.ServiceGroupDisabled));

            return (true, null);
        }
    }
}