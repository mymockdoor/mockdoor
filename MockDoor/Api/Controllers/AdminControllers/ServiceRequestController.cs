using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.Repositories;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.ServiceRequest;
using MockDoor.Shared.Models.Timetravel;

namespace MockDoor.Api.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly ILogger<ServiceRequestController> _logger;
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public ServiceRequestController(ILogger<ServiceRequestController> logger, IServiceRequestRepository serviceRequestRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceRequestRepository = serviceRequestRepository ?? throw new ArgumentNullException(nameof(serviceRequestRepository));
        }

        [HttpGet("list/{microserviceId}")]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetAllForMicroservice(int microserviceId)
        {
            if (microserviceId <= 0)
                return BadRequest(ErrorMessageConstants.MicroserviceId);
            
            _logger.LogInformation("get request for microservice: {MicroserviceId}", microserviceId);
            return Ok(await _serviceRequestRepository.GetAllServiceRequestsForMicroserviceAsync(microserviceId));
        }

        [HttpGet("{serviceRequestId}")]
        public async Task<ActionResult<ServiceRequestDto>> GetById(int serviceRequestId)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);

            return Ok(await _serviceRequestRepository.GetServiceRequest(serviceRequestId));
        }


        [HttpPost("{microserviceId}")]
        public async Task<ActionResult<ServiceRequestDto>> CreateRequest(int microserviceId, [FromBody] ServiceRequestDto? serviceRequest)
        {
            if (microserviceId <= 0)
                return BadRequest(ErrorMessageConstants.MicroserviceId);
            
            if (serviceRequest == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            
            return Ok(await _serviceRequestRepository.CreateServiceRequestAsync(microserviceId, serviceRequest));
        }

        [HttpPut("{serviceRequestId}")]
        public async Task<ActionResult> UpdateRequest(int serviceRequestId, [FromBody] UpdateServiceRequestDto? serviceRequest)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (serviceRequest == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            
            var updatedRequest = await _serviceRequestRepository.UpdateServiceRequest(serviceRequestId, serviceRequest);

            if (updatedRequest == null)
                return NotFound(ErrorMessageConstants.RequestNotFound);
            
            return Ok(updatedRequest);
        }

        [HttpPut("responses/{serviceRequestId}")]
        public async Task<ActionResult> UpdateMockResponses(int serviceRequestId, [FromBody] List<MockResponseDto>? responses)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (responses == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

            var updatedResponses = await _serviceRequestRepository.UpdateMockResponses(serviceRequestId, responses);
            
            if(updatedResponses == null)
                return NotFound(ErrorMessageConstants.RequestNotFound);
            
            return Ok(updatedResponses);
        }

        [HttpPatch("{serviceRequestId}")]
        public async Task<ActionResult> PatchServiceRequest(int serviceRequestId, [FromBody] JsonPatchDocument<UpdateServiceRequestDto>? updatedServiceRequest)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            var serviceRequestDto = await _serviceRequestRepository.GetUpdateServiceRequest(serviceRequestId);

            if (serviceRequestDto == null || updatedServiceRequest == null)
                return NotFound(ErrorMessageConstants.RequestNotFound);
            
            _logger.LogInformation("Patching request");
            updatedServiceRequest.ApplyTo(serviceRequestDto);
            
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(serviceRequestDto, new ValidationContext(serviceRequestDto, null), results, true);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _serviceRequestRepository.UpdateServiceRequest(serviceRequestId, serviceRequestDto);

            return Ok(result);
        }


        [HttpPost("setsimulate/{serviceRequestId}")]
        public async Task<ActionResult> SetSimulate(int serviceRequestId, [FromBody] UpdateTimeTravelDto date)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);

            var serviceRequestDto = await _serviceRequestRepository.GetUpdateServiceRequest(serviceRequestId);

            if (serviceRequestDto == null)
                return NotFound(ErrorMessageConstants.RequestNotFound);

            serviceRequestDto.SimulateTime = date.Time;

            var result = await _serviceRequestRepository.UpdateServiceRequest(serviceRequestId, serviceRequestDto);


            return Ok(result);
        }

        [HttpGet("gettimes/{serviceRequestId}")]
        public async Task<ActionResult> GetTimes(int serviceRequestId)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);

            var serviceRequestDto = await _serviceRequestRepository.GetServiceRequest(serviceRequestId);

            if (serviceRequestDto == null)
                return NotFound(ErrorMessageConstants.RequestNotFound);

            var result = await _serviceRequestRepository.GetMockResponseTimes(serviceRequestId);

            var timeTravelDto = new TimeTravelDto()
            {
                AvailableTimes = result.ToList(),
                CurrentTime = serviceRequestDto.SimulateTime
            };

            return Ok(timeTravelDto);
        }

        [HttpDelete("{serviceRequestId}")]
        public async Task<ActionResult> DeleteRequest(int serviceRequestId)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
                    
            return await _serviceRequestRepository.DeleteRequest(serviceRequestId) ? Ok() : NoContent();
        }
    }
}
