using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.Repositories;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Response;

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
        public async Task<ActionResult> CreateRequest(int requestId, [FromBody] MockResponseDto? response)
        {
            if (requestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (response == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

            var created = await _mockResponseRepository.CreateAsync(requestId, response);
            return created.success ? StatusCode(201, created.result) : BadRequest(ErrorMessageConstants.FailedToCreate);
        }

        [HttpPost("bulk/{requestId}")]
        public async Task<ActionResult> PostBulkCreateForRequest(int requestId, [FromBody] List<MockResponseDto>? responses)
        {
            if (requestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (responses == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

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
        public async Task<ActionResult> UpdateResponseForRequest(int requestId, [FromBody] MockResponseDto? response)
        {
            if (requestId <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (response == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            
            var updatedResponse = await _mockResponseRepository.UpdateMockResponseAsync(requestId, response);
            return Ok(updatedResponse);
        }

        [HttpDelete("{responseId}")]
        public async Task<ActionResult> DeleteResponse(int responseId)
        {
            if (responseId <= 0)
                return BadRequest(ErrorMessageConstants.ResponseId);

            var created = await _mockResponseRepository.DeleteAsync(responseId);
            return created ? Ok() : NoContent();
        }


        [HttpDelete("bulk/{requestid}")]
        public async Task<ActionResult> DeleteBulkCreateForRequest(int requestid, [FromBody] List<MockResponseDto>? responses)
        {
            if (requestid <= 0)
                return BadRequest(ErrorMessageConstants.RequestId);
            
            if (responses == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

            var created = await _mockResponseRepository.DeleteBulkAsync(requestid, responses);
            return created ? Ok() : NoContent();
        }

        [HttpPatch("{mockResponseId}")]
        public async Task<ActionResult> PatchResponse(int mockResponseId, [FromBody] JsonPatchDocument<UpdateMockResponseDto>? updateResponseDto)
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
