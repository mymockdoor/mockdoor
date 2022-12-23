﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.Repositories;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Utility;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace MockDoor.Api.Controllers.AdminControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ValidateNever]
    public class UtilitiesController : ControllerBase
    {
        private const int TimeoutLimit = 3000;

        private readonly ILogger<UtilitiesController> _logger;
        private readonly IBaseRepository _baseRepository;
        private readonly ITenantRepository _tenantRepository;

        public UtilitiesController(ILogger<UtilitiesController> logger, IBaseRepository baseRepository, ITenantRepository tenantRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseRepository = baseRepository ?? throw new ArgumentNullException(nameof(baseRepository));
            _tenantRepository = tenantRepository;
        }

        [HttpPost("testurl")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PingTestResult))]
        public async Task<ActionResult<PingTestResult>> TestUrl([FromBody] TestUrl? test)
        {
            if(test == null)
            {
                return Ok(new PingTestResult()
                {
                    TestUrlResult = TestUrlResult.Unknown,
                    Message = "invalid test request"
                });
            }

            var stopWatch = new Stopwatch();
            try
            {
                if (test.PingOnly)
                {
                    var pingUri = new Uri(test.Url);
                    string host = pingUri.Host;

                    var p = new Ping();

                    stopWatch.Start();
                    var result = p.Send(host, TimeoutLimit);
                    stopWatch.Stop();
                    
                    if (result.Status == IPStatus.Success)
                    {
                        return Ok(new PingTestResult() { 
                            TestUrlResult = TestUrlResult.Passed, 
                            Latency = (int)stopWatch.ElapsedMilliseconds,
                            Message = $"Ping test passed in {stopWatch.ElapsedMilliseconds} ms" 
                        });
                    }

                    return Ok(new PingTestResult()
                    {
                        TestUrlResult = TestUrlResult.Failed,
                        Latency = (int)stopWatch.ElapsedMilliseconds,
                        Message = $"Ping test failed: {result.Status}, {stopWatch.ElapsedMilliseconds} ms"
                    });
                }
                else
                {
                    using var client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(10);

                    stopWatch.Start();
                    var result = await client.GetAsync(test.Url);
                    stopWatch.Stop();

                    if (result.IsSuccessStatusCode)
                    {
                        return Ok(new PingTestResult() { TestUrlResult = TestUrlResult.Passed, Latency = (int)stopWatch.ElapsedMilliseconds, Message = $"Test passed in {stopWatch.ElapsedMilliseconds} ms" });
                    }

                    return Ok(new PingTestResult()
                    {
                        TestUrlResult = TestUrlResult.Failed,
                        Latency = (int)stopWatch.ElapsedMilliseconds,
                        Message = $"Test failed: {result.StatusCode}, {stopWatch.ElapsedMilliseconds} ms"
                    });
                }
            }
            catch (PingException pex)
            {
                if (stopWatch.IsRunning)
                    stopWatch.Stop();

                string errorMessage = pex.Message;

                if(pex.InnerException != null)
                {
                    errorMessage = pex.InnerException.Message;
                }

                return Ok(new PingTestResult()
                {
                    TestUrlResult = TestUrlResult.Failed,
                    Message = $"Ping test failed due to: \"{errorMessage}\""
                });
            }
            catch (Exception ex)
            {
                if (stopWatch.IsRunning)
                    stopWatch.Stop();

                Console.WriteLine(ex.Message);
                return Ok(new PingTestResult()
                {
                    TestUrlResult = TestUrlResult.Failed,
                    Message = $"Test failed due to: \"{ex.Message}\""
                });
            }
        }

        [HttpGet("database/export")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<FileContentResult> ExportDatabaseAsFile()
        {
            _logger.LogInformation("exporting database");

            var json = JsonConvert.SerializeObject(await _baseRepository.ExportDatabaseToJson(), Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            
            return File(Encoding.UTF8.GetBytes(json), "application/json", "mockdoor-backup.json");
        }
        
        [HttpGet("database/exportasjson")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(FullDatabaseDto))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<FullDatabaseDto>> ExportDatabaseAsJson()
        {
            _logger.LogInformation("exporting database");
            return Ok(await _baseRepository.ExportDatabaseToJson());
        }

        [HttpPost("database/import")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(BadRequestResultDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult> ImportDatabaseAsJson([FromBody] FullDatabaseDto? import)
        {
            if (import == null)
            {
                return BadRequest(ErrorMessageConstants.InvalidOrMissingRequestBody);
            }
            var existingTenantDetails = await _tenantRepository.GetAllTakenTenantNameAndPathsAsync();
            
            var results = new List<ValidationResult>();
            bool isValid = GeneralHelper.TryValidateFullObject(import, new ValidationContext(import,
                new Dictionary<object, object?>()
                {
                    { "Path", existingTenantDetails.Select(et => et.Path) },
                    { "Name", existingTenantDetails.Select(et => et.Name) }
                }), results);

            if (!isValid)
                return BadRequest(results.ToBadRequestResult());
            
            var imported = await _baseRepository.ImportDatabase(import, false);

            if (!imported)
            {
                return BadRequest(ErrorMessageConstants.TenantNotFound);
            }
            
            return Ok();
        }
    }
}
