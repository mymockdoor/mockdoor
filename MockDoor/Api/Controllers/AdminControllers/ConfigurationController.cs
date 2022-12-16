using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockDoor.Abstractions.ConfigurationServices;
using MockDoor.Shared.Models.Configuration;
using MockDoor.Shared.Models.Utility;

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
        public async Task<ActionResult> ApplyMigrations()
        {
            await _databaseConfigurationService.ApplyMigrationsAsync(_deploymentConfiguration
                .DatabaseConfig.ConnectionString);
            return Ok();
        }
    }
}
