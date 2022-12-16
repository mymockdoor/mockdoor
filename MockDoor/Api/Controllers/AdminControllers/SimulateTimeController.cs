using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.MockServices;
using MockDoor.Shared;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Models.Timetravel;

namespace MockDoor.Api.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulateTimeController : ControllerBase
    {

        private readonly ILogger<SimulateTimeController> _logger;
        private readonly ISimulateTimeService _simulateTimeService;

        public SimulateTimeController(ILogger<SimulateTimeController> logger, ISimulateTimeService simulateTimeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _simulateTimeService = simulateTimeService ?? throw new ArgumentNullException(nameof(simulateTimeService));
        }

        [HttpPost("setsimulate/{id}")]
        public async Task<ActionResult> SetSimulateAsync(int id, [FromBody] UpdateTimeTravelDto? updateTimeTravelDto)
        {
            if (id <= 0)
                return BadRequest(ErrorMessageConstants.ScopeId);

            if (updateTimeTravelDto == null)
            {
                return BadRequest(ErrorMessageConstants.SimulateTimeModelWasNotProvided);
            }
            _logger.LogInformation("Setting simulation time {Time}, {Scope}", updateTimeTravelDto.Time, updateTimeTravelDto.Scope.ToString());
            
            if (await _simulateTimeService.SetSimulateTime(updateTimeTravelDto, id))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("times/{scope}/{id}")]
        public async Task<ActionResult> GetTimesAsync(TimeTravelScope scope, int id)
        {
            if (id <= 0)
                return BadRequest(ErrorMessageConstants.ScopeId);
            
            var serviceRequestDto = await _simulateTimeService.GetTimes(scope, id);

            if (serviceRequestDto == null)
                return NotFound();

            return Ok(serviceRequestDto);
        }
    }
}
