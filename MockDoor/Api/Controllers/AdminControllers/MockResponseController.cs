using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.Repositories;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Response;
using MockDoor.Shared.Models.Utility;
using Swashbuckle.AspNetCore.Annotations;

namespace MockDoor.Api.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockResponseController : ControllerBase
    {

        private readonly ILogger<MockResponseController> _logger;
        private readonly IMockResponseRepository _mockResponseRepository;

        public MockResponseController(ILogger<MockResponseController> logger, IMockResponseRepository mockResponseRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mockResponseRepository = mockResponseRepository ?? throw new ArgumentNullException(nameof(mockResponseRepository));
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(MockResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<MockResponseDto>> Get(int id)
        {
            if (id <= 0)
                return BadRequest(ErrorMessageConstants.ResponseId);
            
            var response = await _mockResponseRepository.GetMockResponseAsync(id);

            if (response == null)
                return NotFound(ErrorMessageConstants.ResponseNotFound);

            return Ok(response);
        }

        [HttpPost("{requestId}")]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(MockResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<MockResponseDto>> CreateResponse(int requestId, [FromBody] MockResponseDto? response)
        {
            if (requestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (response == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

            var results = new List<ValidationResult>();

            bool isValid = GeneralHelper.TryValidateFullObject(response, new ValidationContext(response, null), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());
            
            var created = await _mockResponseRepository.CreateAsync(requestId, response);
            return created.success ? StatusCode(201, created.result) : BadRequest(ErrorMessageConstants.FailedToCreate);
        }

        [HttpPost("bulk/{requestId}")]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(List<MockResponseDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<List<MockResponseDto>>> PostBulkCreateForRequest(int requestId, [FromBody] List<MockResponseDto>? responses)
        {
            if (requestId <= 0)
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
            
            var created = await _mockResponseRepository.CreateBulkAsync(requestId, responses);
            
            if (created.success)
            {
                return StatusCode(201, created.result);
            }
            else
            {
                if (created.result == null)
                    return NotFound(ErrorMessageConstants.RequestNotFound);
                
                return BadRequest(ErrorMessageConstants.FailedToCreate);
            }
        }

        [HttpPut("{requestId}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(MockResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<MockResponseDto>> UpdateResponseForRequest(int requestId, [FromBody] MockResponseDto? response)
        {
            if (requestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (response == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            
            var updatedResponse = await _mockResponseRepository.UpdateMockResponseAsync(requestId, response);

            if (updatedResponse == null)
            {
                return NotFound(ErrorMessageConstants.ResponseNotFound);
            }
            
            return Ok(updatedResponse);
        }

        [HttpDelete("{responseId}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult> DeleteResponse(int responseId)
        {
            if (responseId <= 0)
                return BadRequest(ErrorMessageConstants.ResponseId);

            var deleted = await _mockResponseRepository.DeleteAsync(responseId);
            return deleted ? Ok() : NoContent();
        }


        [HttpDelete("bulk/{requestid}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult> DeleteBulkCreateForRequest(int requestid, [FromBody] List<MockResponseDto>? responses)
        {
            if (requestid <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (responses == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

            var deleted = await _mockResponseRepository.DeleteBulkAsync(requestid, responses);
            return deleted ? Ok() : NoContent();
        }

        [HttpPatch("{mockResponseId}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(UpdateMockResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<UpdateMockResponseDto>> PatchResponse(int mockResponseId, [FromBody] JsonPatchDocument<UpdateMockResponseDto>? updateResponseDto)
        {
            if (mockResponseId <= 0)
                return BadRequest(ErrorMessageConstants.ResponseId);
            
            if (updateResponseDto == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            
            var response = await _mockResponseRepository.GetUpdateMockResponseAsync(mockResponseId);

            if (response == null)
                return NotFound(ErrorMessageConstants.ResponseNotFound);
            
            _logger.LogInformation("patching response");
            updateResponseDto.ApplyTo(response);
            
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(response, new ValidationContext(response, null), results, true);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());

            var result = await _mockResponseRepository.PatchMockResponseAsync(mockResponseId, response);

            if (result == null)
                return NotFound(ErrorMessageConstants.ResponseNotFound);
            
            return Ok(result);
        }
    }
}
