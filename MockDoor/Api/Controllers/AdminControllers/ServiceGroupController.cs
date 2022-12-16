using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.Repositories;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.ServiceGroup;

namespace MockDoor.Api.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceGroupController : ControllerBase
    {

        private readonly ILogger<ServiceGroupController> _logger;
        private readonly IServiceGroupRepository _serviceGroupRepository;

        public ServiceGroupController(ILogger<ServiceGroupController> logger, IServiceGroupRepository serviceGroupRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceGroupRepository = serviceGroupRepository ?? throw new ArgumentNullException(nameof(serviceGroupRepository));
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<BaseServiceGroupDto>>> Get()
        {
            _logger.LogInformation("Get Service list called");
            var serviceGroups = await _serviceGroupRepository.GetServiceGroups();
            
            return Ok(serviceGroups);
        }

        [HttpGet("list/{tenantId}")]
        public async Task<ActionResult<ServiceGroupOverviewCollection>> Get(int tenantId)
        {
            if (tenantId <= 0)
                return BadRequest(ErrorMessageConstants.TenantId);
            
            var serviceGroups = await _serviceGroupRepository.GetServiceGroupsByTenantId(tenantId);

            if (serviceGroups == null)
                return NotFound(ErrorMessageConstants.TenantNotFound);

            return Ok(serviceGroups);
        }

        [HttpGet("findbyfullpath/{tenantpath}/{servicegrouppath}")]
        public async Task<ActionResult<BasicServiceGroupDto>> GetByFullPath(string tenantpath, string servicegrouppath)
        {
            if (string.IsNullOrWhiteSpace(tenantpath))
                return BadRequest(ErrorMessageConstants.TenantPath);
            
            if (string.IsNullOrWhiteSpace(servicegrouppath))
                return BadRequest(ErrorMessageConstants.ServiceGroupPath);
            
            var serviceGroupId = await _serviceGroupRepository.GetServiceGroupId(tenantpath, servicegrouppath);

            if (serviceGroupId == null)
                return NotFound();

            var serviceGroup = await _serviceGroupRepository.GetServiceGroupById(serviceGroupId.Value);

            return Ok(serviceGroup);
        }

        [HttpGet("findbypath/{tenant}/{servicegrouppath}")]
        public async Task<ActionResult<BasicServiceGroupDto>> GetByFullPath(int tenant, string servicegrouppath)
        {
            if (tenant <= 0)
                return BadRequest(ErrorMessageConstants.TenantId);
            
            if (string.IsNullOrWhiteSpace(servicegrouppath))
                return BadRequest(ErrorMessageConstants.ServiceGroupPath);
            
            var serviceGroupId = await _serviceGroupRepository.GetServiceGroupId(tenant, servicegrouppath);

            if (serviceGroupId == null)
                return NotFound(ErrorMessageConstants.ServiceGroupNotFound);

            var serviceGroup = await _serviceGroupRepository.GetServiceGroupById(serviceGroupId.Value);

            return Ok(serviceGroup);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BasicServiceGroupDto>> GetServiceGroupById(int id)
        {
            if (id <= 0)
                return BadRequest(ErrorMessageConstants.ServiceGroupId);
            
            var serviceGroup = await _serviceGroupRepository.GetServiceGroupById(id);

            return serviceGroup == null ? NotFound(ErrorMessageConstants.ServiceGroupNotFound) : Ok(serviceGroup);
        }

        [HttpPost("{tenantId}")]
        public async Task<ActionResult<BaseServiceGroupDto>> CreateServiceGroup(int tenantId, [FromBody] BaseServiceGroupDto? newServiceGroup)
        {
            if (tenantId <= 0)
                return BadRequest(ErrorMessageConstants.TenantId);
            
            if (newServiceGroup == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

            if (newServiceGroup.Id != 0)
                return BadRequest(ErrorMessageConstants.NewServiceGroupId);

            if (newServiceGroup.TenantId > 0 && newServiceGroup.TenantId != tenantId)
                return BadRequest(ErrorMessageConstants.IdMissMatch);

            var results = new List<ValidationResult>();

            var existingGroupPaths = await _serviceGroupRepository.GetAllServiceGroupNameAndPathsForTenant(tenantId);

            bool isValid = GeneralHelper.TryValidateFullObject(newServiceGroup, new ValidationContext(newServiceGroup, 
                new Dictionary<object, object?>()
                {
                    { "Path", existingGroupPaths.Select(sg => sg.Path) },
                    { "Name", existingGroupPaths.Select(sg => sg.Name)}
                }), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());

            return Ok(await _serviceGroupRepository.CreateServiceGroup(newServiceGroup));
        }

        [HttpPut("{serviceGroupId}")]
        public async Task<ActionResult> UpdateServiceGroup(int serviceGroupId, [FromBody] BaseServiceGroupDto? updatedServiceGroup)
        {
            if (serviceGroupId <= 0)
                return BadRequest(ErrorMessageConstants.ServiceGroupId);
            
            if (updatedServiceGroup == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);

            if (updatedServiceGroup.Id != serviceGroupId)
                return BadRequest(ErrorMessageConstants.IdMissMatch);

            var results = new List<ValidationResult>();

            var existingGroupPaths = await _serviceGroupRepository.GetAllServiceGroupNameAndPathsForTenant(updatedServiceGroup.TenantId, serviceGroupId);

            bool isValid = GeneralHelper.TryValidateFullObject(updatedServiceGroup, new ValidationContext(updatedServiceGroup, 
                new Dictionary<object, object?>()
                {
                    { "Path", existingGroupPaths.Select(sg => sg.Path) },
                    { "Name", existingGroupPaths.Select(sg => sg.Name)}
                }), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());

            return Ok(await _serviceGroupRepository.UpdateServiceGroupBaseValues(updatedServiceGroup));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteServiceGroup(int id)
        {
            if (id <= 0)
                return BadRequest(ErrorMessageConstants.ServiceGroupId);

            if (await _serviceGroupRepository.DeleteServiceGroup(id))
                return Ok();

            return NoContent();
        }
    }
}
