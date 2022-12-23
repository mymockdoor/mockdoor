using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.Repositories;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.ServiceRequest;
using MockDoor.Shared.Models.Timetravel;
using MockDoor.Shared.Models.Utility;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<ServiceRequestDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetAllForMicroservice(int microserviceId)
        {
            if (microserviceId <= 0)
                return BadRequest(ErrorMessageConstants.MicroserviceId);
            
            _logger.LogInformation("get request for microservice: {MicroserviceId}", microserviceId);
            return Ok(await _serviceRequestRepository.GetAllServiceRequestsForMicroserviceAsync(microserviceId));
        }

        [HttpGet("{serviceRequestId}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ServiceRequestDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<ServiceRequestDto>> GetById(int serviceRequestId)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);

            var serviceRequest = await _serviceRequestRepository.GetServiceRequest(serviceRequestId);

            if (serviceRequest == null)
                return NotFound(ErrorMessageConstants.RequestNotFound);
            
            return Ok(serviceRequest);
        }


        [HttpPost("{microserviceId}")]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(ServiceRequestDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        public async Task<ActionResult<ServiceRequestDto>> CreateRequest(int microserviceId, [FromBody] ServiceRequestDto? serviceRequest)
        {
            if (microserviceId <= 0)
                return BadRequest(ErrorMessageConstants.MicroserviceId);
            
            if (serviceRequest == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

            var results = new List<ValidationResult>();

            bool isValid = GeneralHelper.TryValidateFullObject(serviceRequest, new ValidationContext(serviceRequest, null), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());
            
            var createdRequest =
                await _serviceRequestRepository.CreateServiceRequestAsync(microserviceId, serviceRequest);

            if (createdRequest == null)
                return NotFound(ErrorMessageConstants.MicroserviceNotFound);
            
            return StatusCode(201, createdRequest);
        }

        [HttpPut("{serviceRequestId}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UpdateServiceRequestDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<UpdateServiceRequestDto>> UpdateRequest(int serviceRequestId, [FromBody] UpdateServiceRequestDto? serviceRequest)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (serviceRequest == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            
            var results = new List<ValidationResult>();

            bool isValid = GeneralHelper.TryValidateFullObject(serviceRequest, new ValidationContext(serviceRequest, null), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());
            
            var updatedRequest = await _serviceRequestRepository.UpdateServiceRequest(serviceRequestId, serviceRequest);

            if (updatedRequest == null)
                return NotFound(ErrorMessageConstants.RequestNotFound);
            
            return Ok(updatedRequest);
        }

        [HttpPut("responses/{serviceRequestId}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(List<MockResponseDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult> UpdateMockResponses(int serviceRequestId, [FromBody] List<MockResponseDto>? responses)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (responses == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

            var results = new List<ValidationResult>();

            foreach (var response in responses)
            {
                GeneralHelper.TryValidateFullObject(response, new ValidationContext(response, null), results);
            }

            if (results.Any())
                return BadRequest(results.ToBadRequestResult());
            
            var updatedResponses = await _serviceRequestRepository.UpdateMockResponses(serviceRequestId, responses);
            
            if(updatedResponses == null)
                return NotFound(ErrorMessageConstants.RequestNotFound);
            
            return Ok(updatedResponses);
        }

        [HttpPatch("{serviceRequestId}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UpdateServiceRequestDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<UpdateServiceRequestDto>> PatchServiceRequest(int serviceRequestId, [FromBody] JsonPatchDocument<UpdateServiceRequestDto>? updatedServiceRequest)
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

            var updatedRequest = await _serviceRequestRepository.UpdateServiceRequest(serviceRequestId, serviceRequestDto);

            if (updatedRequest == null)
                return NotFound(ErrorMessageConstants.MicroserviceNotFound);
            
            return Ok(updatedRequest);
        }

        [HttpDelete("{serviceRequestId}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult> DeleteRequest(int serviceRequestId)
        {
            if (serviceRequestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
                    
            return await _serviceRequestRepository.DeleteRequest(serviceRequestId) ? Ok() : NoContent();
        }
    }
}
