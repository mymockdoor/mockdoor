using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockDoor.Abstractions.ConfigurationServices;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Configuration;
using MockDoor.Shared.Models.Utility;
using Swashbuckle.AspNetCore.Annotations;

namespace MockDoor.Api.Controllers.AdminControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {

        private readonly ILogger<ConfigurationController> _logger;
        private readonly DeploymentConfiguration _deploymentConfiguration;
        private readonly IDatabaseConfigurationService _databaseConfigurationService;

        public ConfigurationController(ILogger<ConfigurationController>? logger, IOptions<DeploymentConfiguration>? deploymentConfigurationOptions,  IDatabaseConfigurationService databaseConfigurationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _deploymentConfiguration = deploymentConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(deploymentConfigurationOptions));
            _databaseConfigurationService = databaseConfigurationService ?? throw new ArgumentNullException(nameof(databaseConfigurationService));
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(DeploymentConfiguration))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<DeploymentConfiguration>> GetAsync()
        {
            _deploymentConfiguration.SqlConnectionStatus =
                await _databaseConfigurationService.DoesConnectionStringWorkAsync(_deploymentConfiguration
                    .DatabaseConfig.ConnectionString);

            if (_deploymentConfiguration.SqlConnectionStatus == ConnectionStringStatus.Success)
                _deploymentConfiguration.PendingMigrations =
                    await _databaseConfigurationService.GetPendingMigrationsAsync();
            else
                _deploymentConfiguration.PendingMigrations = _databaseConfigurationService.GetAllMigrations();
            
            var response = _deploymentConfiguration.CopyTo(new DeploymentConfiguration());
            
            if (!string.IsNullOrWhiteSpace(response.DatabaseConfig?.ConnectionString) && !(response.Debug ?? true))
            {
                response.DatabaseConfig.ConnectionString = "*****";
            }

            return Ok(response);
        }

        [HttpPost("testconnection")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ConnectionStringTestResult))]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<ConnectionStringTestResult>> TestConnection()
        {
            _logger.LogInformation("testing database connection");
            if (_deploymentConfiguration.Debug ?? false)
            {
                using StreamReader reader = new(Request.Body, Encoding.UTF8);
                var result = await _databaseConfigurationService.TestConnectionStringWorkAsync(await reader.ReadToEndAsync());

                return Ok(result);
            }
            return NotFound(); 
        }

        [HttpPost("applymigrations")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult> ApplyMigrations()
        {
            try
            {
                await _databaseConfigurationService.ApplyMigrationsAsync(_deploymentConfiguration
                    .DatabaseConfig.ConnectionString);
            }
            catch (SqlException sqlException)
            {
                return BadRequest(sqlException.ToBadRequestResult("ApplyMigration"));
            }
            catch (Exception)
            {
                return BadRequest("Failed for unexpected reason".ToBadRequestResult());
            }

            return Ok();
        }
    }
}
