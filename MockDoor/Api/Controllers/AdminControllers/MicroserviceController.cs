#nullable enable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.Repositories;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.Utility;

namespace MockDoor.Api.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MicroserviceController : ControllerBase
    {

        private readonly ILogger<MicroserviceController> _logger;
        private readonly IMicroserviceRepository _microserviceRepository;

        public MicroserviceController(ILogger<MicroserviceController>? logger, IMicroserviceRepository? microserviceRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _microserviceRepository = microserviceRepository ?? throw new ArgumentNullException(nameof(microserviceRepository));
        }

        [HttpGet("{microserviceId}")]
        public async Task<ActionResult<MicroserviceResultDto>> Get(int microserviceId)
        {
            _logger.LogInformation("Getting microservice: {MicroserviceId}", microserviceId);

            if (microserviceId <= 0)
                return BadRequest(ErrorMessageConstants.MicroserviceId);
            
            var service =  await _microserviceRepository.GetMicroserviceById(microserviceId);

            if (service == null)
                return NotFound(ErrorMessageConstants.MicroserviceNotFound);

            return Ok(service);
        }

        [HttpGet("findbypath/{serviceGroupId}/{microservicePath}")]
        public async Task<ActionResult<MicroserviceResultDto>> Get(int serviceGroupId, string? microservicePath)
        {
            if (serviceGroupId <= 0)
                return BadRequest(ErrorMessageConstants.ServiceGroupId);
            
            if (string.IsNullOrWhiteSpace(microservicePath))
                return BadRequest(ErrorMessageConstants.MicroservicePath);
            
            var service = await _microserviceRepository.GetMicroservice(serviceGroupId, microservicePath);

            if(service == null)
                return NotFound(ErrorMessageConstants.MicroserviceNotFound);

            return Ok(service);
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<MicroserviceResultDto>>> GetAllMicroservices()
        {
            return Ok(await _microserviceRepository.GetAllMicroservices());
        }

        [HttpGet("searchresultlist")]
        public async Task<ActionResult<IEnumerable<MicroserviceSearchResultDto>>> GetAllMicroserviceSearchResults()
        {
            return Ok(await _microserviceRepository.GetAllMicroserviceSearchResults());
        }

        [HttpGet("list/{serviceGroupId}")]
        public async Task<ActionResult<IEnumerable<MicroserviceResultDto>>> GetMicroservicesForServiceGroup(int serviceGroupId)
        {
            if (serviceGroupId <= 0)
                return BadRequest(ErrorMessageConstants.ServiceGroupId);
            
            return Ok(await _microserviceRepository.GetAllMicroservicesForServiceGroup(serviceGroupId));
        }

        [HttpGet("parents/{microserviceId}")]
        public async Task<ActionResult<MicroserviceParentIds>> GetParentIds(int microserviceId)
        {
            if (microserviceId <= 0)
                return BadRequest(ErrorMessageConstants.MicroserviceId);
            
            var microserviceParentIds = await _microserviceRepository.GetParentIds(microserviceId);

            if(microserviceParentIds == null)
                return NotFound(ErrorMessageConstants.MicroserviceNotFound);

            return microserviceParentIds;
        }

        [HttpPost("{serviceGroupId}")]
        public async Task<ActionResult<MicroserviceResultDto>> CreateMicroservice(int serviceGroupId, [FromBody] MicroserviceResultDto? newMicroservice)
        {
            if (serviceGroupId <= 0)
                return BadRequest(ErrorMessageConstants.ServiceGroupId);
            
            if (newMicroservice == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            
            if (newMicroservice.Id != 0)
                return BadRequest(ErrorMessageConstants.NewMicroserviceId);

            if (string.IsNullOrWhiteSpace(newMicroservice.Name))
                return BadRequest(ErrorMessageConstants.MicroserviceName);
            
            if (string.IsNullOrWhiteSpace(newMicroservice.Path))
                return BadRequest(ErrorMessageConstants.MicroservicePath);

            if (newMicroservice.ProxyMode && string.IsNullOrWhiteSpace(newMicroservice.TargetUrl))
                return BadRequest(ErrorMessageConstants.MicroserviceTargetUrl);

            if (newMicroservice.RegisteredServiceGroupId > 0 && newMicroservice.RegisteredServiceGroupId != serviceGroupId)
                return BadRequest(ErrorMessageConstants.IdMissMatch);

            var results = new List<ValidationResult>();

            var existingPaths = await _microserviceRepository.GetAllMicroservicePathAndNamesForServiceGroup(serviceGroupId);

            bool isValid = GeneralHelper.TryValidateFullObject(newMicroservice, new ValidationContext(newMicroservice,
                new Dictionary<object, object?>()
                {
                    { "Path", existingPaths.Select(ep => ep.Path) },
                    { "Name", existingPaths.Select(ep => ep.Name) }
                }), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());

            newMicroservice.RegisteredServiceGroupId = serviceGroupId;
            var createdMicroservice = await _microserviceRepository.CreateMicroservice(newMicroservice);

            if (createdMicroservice == null)
                return NotFound(ErrorMessageConstants.ServiceGroupNotFound);
            
            return Ok(createdMicroservice);
        }

        [HttpPut("{microserviceId}")]
        public async Task<ActionResult> UpdateMicroservice(int microserviceId, [FromBody] MicroserviceResultDto? updatedMicroservice)
        {
            if (microserviceId <= 0)
                return BadRequest(ErrorMessageConstants.MicroserviceId);
            
            if (updatedMicroservice == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            
            if (updatedMicroservice.Id != microserviceId)
                return BadRequest(ErrorMessageConstants.IdMissMatch);

            if (string.IsNullOrWhiteSpace(updatedMicroservice.Name))
                return BadRequest(ErrorMessageConstants.MicroserviceName);
            
            if (string.IsNullOrWhiteSpace(updatedMicroservice.Path))
                return BadRequest(ErrorMessageConstants.MicroservicePath);

            if (updatedMicroservice.ProxyMode && string.IsNullOrWhiteSpace(updatedMicroservice.TargetUrl))
                return BadRequest(ErrorMessageConstants.MicroserviceTargetUrl);
            
            var results = new List<ValidationResult>();

            var existingPaths = await _microserviceRepository.GetAllMicroservicePathAndNamesForServiceGroup(updatedMicroservice.RegisteredServiceGroupId, microserviceId);

            bool isValid = GeneralHelper.TryValidateFullObject(updatedMicroservice, new ValidationContext(updatedMicroservice, 
                new Dictionary<object, object?>()
                {
                    { "Path", existingPaths.Select(ep => ep.Path) },
                    { "Name", existingPaths.Select(ep => ep.Name) }
                }), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());

            return await _microserviceRepository.UpdateMicroservice(updatedMicroservice) ? Ok() : NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(ErrorMessageConstants.MicroserviceId);

            if (await _microserviceRepository.DeleteMicroservice(id))
                return Ok();

            return NoContent();
        }
    }
}
