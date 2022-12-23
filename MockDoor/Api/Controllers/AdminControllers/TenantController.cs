using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.Repositories;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.General;
using MockDoor.Shared.Models.Tenant;
using MockDoor.Shared.Models.Utility;
using Swashbuckle.AspNetCore.Annotations;

namespace MockDoor.Api.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {

        private readonly ILogger<TenantController> _logger;
        private readonly ITenantRepository _tenantRepository;

        public TenantController(ILogger<TenantController> logger, ITenantRepository tenantRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
        }

        [HttpGet("list")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TenantNameList))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<TenantNameList>> GetNameList([FromQuery] int skip = 0, [FromQuery] int take = 1000)
        {            
            var tenantList = await _tenantRepository.GetAllTenantsListAsync(skip, take);
            var tenantNameList = new TenantNameList()
            {
                Tenants = tenantList.Tenants.Select(t => new EntityKeyName() { Id = t.Id, Name = t.Name }).ToList()
            };
            
            return Ok(tenantNameList);
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(TenantListDto))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<TenantListDto>> Get([FromQuery]int skip = 0, [FromQuery]int take = 1000)
        {
            _logger.LogInformation("Get tenant list: skip={Skip}, take={Take}", skip, take);
            return Ok(await _tenantRepository.GetAllTenantsListAsync(skip, take));
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseTenantDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<BaseTenantDto>> GetTenantById(int id)
        {
            if (id <= 0)
                return BadRequest(ErrorMessageConstants.TenantId);
            
            var tenant = await _tenantRepository.GetTenantByIdAsync(id);

            return tenant != null ? Ok(tenant) : NotFound();
        }

        [HttpGet("findbyname/{tenantName}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseTenantDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<BaseTenantDto>> GetByName(string tenantName)
        {
            if (string.IsNullOrWhiteSpace(tenantName))
                return BadRequest(ErrorMessageConstants.TenantName);
            
            var tenant = await _tenantRepository.GetTenantByNameAsync(tenantName);

            return tenant != null ? Ok(tenant) : NotFound();
        }

        [HttpGet("findbypath/{path}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseTenantDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<BaseTenantDto>> GetByPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return BadRequest(ErrorMessageConstants.TenantPath);
            
            var tenant = await _tenantRepository.GetTenantByPathAsync(path);

            return tenant != null ? Ok(tenant) : NotFound();
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, Type = typeof(BaseTenantDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<BaseTenantDto>> CreateTenant([FromBody] BaseTenantDto newTenant)
        {
            if (newTenant.Id != 0)
                return BadRequest(ErrorMessageConstants.NewTenantId);

            var results = new List<ValidationResult>();

            var existingTenantDetails = await _tenantRepository.GetAllTakenTenantNameAndPathsAsync();
            
            bool isValid = GeneralHelper.TryValidateFullObject(newTenant, new ValidationContext(newTenant, 
                new Dictionary<object, object?>()
                        {
                            { "Path", existingTenantDetails.Select(et => et.Path) },
                            { "Name", existingTenantDetails.Select(et => et.Name) }
                        }), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());

            return StatusCode(201, await _tenantRepository.CreateTenantAsync(newTenant));
        }

        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(BaseTenantDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult> UpdateTenant(int id, [FromBody]BaseTenantDto? updatedTenant)
        {
            if (id == 0 )
                return BadRequest(ErrorMessageConstants.TenantId);

            if (updatedTenant == null)
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            
            if (updatedTenant.Id > 0 && updatedTenant.Id != id)
                return BadRequest(ErrorMessageConstants.IdMissMatch);
            
            var results = new List<ValidationResult>();

            updatedTenant.Id = id;
            var existingTenantDetails = await _tenantRepository.GetAllTakenTenantNameAndPathsAsync(updatedTenant.Id);

            bool isValid = GeneralHelper.TryValidateFullObject(updatedTenant, new ValidationContext(updatedTenant, 
                new Dictionary<object, object?>()
                {
                    { "Path", existingTenantDetails.Select(et => et.Path) },
                    { "Name", existingTenantDetails.Select(et => et.Name) }
                }), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());

            if (await _tenantRepository.UpdateTenantBaseValuesAsync(updatedTenant))
            {
                return Ok(updatedTenant);
            }

            return NotFound(ErrorMessageConstants.TenantNotFound);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult> DeleteTenant(int id)
        {
            if (id <= 0)
                return BadRequest(ErrorMessageConstants.TenantId);
            
            var deleted = await _tenantRepository.DeleteTenantAsync(id);

            if (!deleted)
                return NoContent();

            return Ok();
        }
    }
}
