using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MockDoor.Abstractions.Repositories;
using MockDoor.Data.Contexts;
using MockDoor.Data.Models;
using MockDoor.Shared.Constants;
using MockDoor.Shared.Models.Configuration;
using MockDoor.Shared.Models.Timetravel;
using MockDoor.Shared.Models.Utility;

namespace MockDoor.Data.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        private readonly MockDoorMainContext _context;
        private readonly DeploymentConfiguration _deploymentConfiguration;
        private readonly TenantMapper _tenantMapper = new TenantMapper();

        public BaseRepository(MockDoorMainContext context, IOptions<DeploymentConfiguration> deploymentOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _deploymentConfiguration = deploymentOptions?.Value ?? throw new ArgumentNullException(nameof(deploymentOptions));
        }

        #region Set Simulation Time
        public async Task<bool> SetSimulateTimeOnRequest(DateTime? time, int id)
        {
            var serviceRequest = await _context.ServiceRequests.FirstOrDefaultAsync(rr => rr.ID == id);

            if (serviceRequest == null)
                return false;

            serviceRequest.SimulateTime = time;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SetSimulateTimeOnMicroservice(DateTime? time, int id)
        {
            var microservice = await _context.Microservices.FirstOrDefaultAsync(m => m.ID == id);

            if (microservice == null)
                return false;

            microservice.SimulateTime = time;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SetSimulateTimeOnServiceGroup(DateTime? time, int id)
        {
            var serviceGroup = await _context.ServiceGroups.FirstOrDefaultAsync(sg => sg.ID == id);

            if (serviceGroup == null)
                return false;

            serviceGroup.SimulateTime = time;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SetSimulateTimeOnTenant(DateTime? time, int id)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.ID == id);

            if (tenant == null)
                return false;

            tenant.SimulateTime = time;

            await _context.SaveChangesAsync();

            return true;
        }
        #endregion

        #region Get Times
        public async Task<TimeTravelDto> GetRequestTimes(int id)
        {
            var serviceRequestDto = await _context.ServiceRequests.Include(sr => sr.MockResponses).FirstOrDefaultAsync(sr => sr.ID == id);

            if (serviceRequestDto == null)
                return null;

            var result = serviceRequestDto.MockResponses.Select(rr => rr.CreatedUtc).ToList();

            return new TimeTravelDto()
            {
                AvailableTimes = result.ToList(),
                CurrentTime = serviceRequestDto.SimulateTime
            };
        }

        public async Task<TimeTravelDto> GetMicroserviceTimes(int id)
        {
            var microservice = await _context.Microservices.FirstOrDefaultAsync(m => m.ID == id);
            var serviceRequests = _context.ServiceRequests.Include(sr => sr.MockResponses).Where(sr => sr.MicroserviceID == id);

            var result = serviceRequests.SelectMany(sr => sr.MockResponses)
                                            .Select(mr => mr.CreatedUtc).ToList();

            return new TimeTravelDto()
            {
                AvailableTimes = result.ToList(),
                CurrentTime = microservice.SimulateTime
            };
        }

        public async Task<TimeTravelDto> GetServiceGroupTimes(int id)
        {
            var serviceGroup = await _context.ServiceGroups.FirstOrDefaultAsync(m => m.ID == id);
            var microservices = _context.Microservices.Include(m => m.ServiceRequests).ThenInclude(sr => sr.MockResponses).Where(m => m.ServiceGroupID == id);

            var result = microservices.SelectMany(m => m.ServiceRequests)
                                        .SelectMany(sr => sr.MockResponses)
                                        .Select(mr => mr.CreatedUtc).ToList();

            return new TimeTravelDto()
            {
                AvailableTimes = result.ToList(),
                CurrentTime = serviceGroup.SimulateTime
            };
        }

        public async Task<TimeTravelDto> GetTenantGroupTimes(int id)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.ID == id);
            var serviceGroups = _context.ServiceGroups.Include(t => t.Microservices).ThenInclude(m => m.ServiceRequests).ThenInclude(sr => sr.MockResponses).Where(sg => sg.TenantID == id);

            var result = serviceGroups.SelectMany(sg => sg.Microservices)
                                        .SelectMany(m => m.ServiceRequests)
                                        .SelectMany(sr => sr.MockResponses)
                                        .Select(mr => mr.CreatedUtc).ToList();

            return new TimeTravelDto()
            {
                AvailableTimes = result.ToList(),
                CurrentTime = tenant.SimulateTime
            };
        }
        #endregion

        public async Task<FullDatabaseDto> ExportDatabaseToJson()
        {
            var fullDatabase = new FullDatabaseDto
            {
                DatabaseType = _deploymentConfiguration.DatabaseConfig.Provider.ToString(),
                CodeVersion = SharedConstants.MockdoorVersion,
                AppliedMigrations = await _context.Database.GetAppliedMigrationsAsync()
            };

            var tenants = _context.Tenants.Include(t => t.ServiceGroups).ToList();
            
            foreach (var tenant in tenants)   
            {
                foreach (var serviceGroup in tenant.ServiceGroups)
                {
                    serviceGroup.Microservices = _context.Microservices
                        .Include(ms => ms.Headers)
                        .Where(ms => ms.ServiceGroupID == serviceGroup.ID)
                        .ToList();
                    
                    foreach (var microservice in serviceGroup.Microservices)
                    {
                        microservice.ServiceRequests = _context.ServiceRequests
                                                                .Include(sr => sr.RequestHeaders)
                                                                .Include(sr => sr.QueryParameters)
                                                                .Where(sr => sr.MicroserviceID == microservice.ID)
                                                                .ToList();
                        

                        foreach (var serviceRequest in microservice.ServiceRequests)
                        {
                            serviceRequest.MockResponses = _context.MockResponses
                                                            .Include(mr => mr.Headers)
                                                            .Where(mr => mr.ServiceRequestId == serviceRequest.ID)
                                                            .ToList();
                            
                            //clear ids to zero
                            serviceRequest.MockResponses.ForEach(mr =>
                            {
                                mr.ID = 0;
                                mr.Headers?.ForEach(h => h.ID = 0);
                            });
                        }
                        
                        //clear ids to zero
                        microservice.ServiceRequests.ForEach(mr =>
                        {
                            mr.ID = 0;
                            mr.RequestHeaders?.ForEach(h => h.ID = 0);
                            mr.QueryParameters?.ForEach(h => h.Id = 0);
                        });
                    }
                    
                    //clear ids to zero
                    serviceGroup.Microservices.ForEach(mr =>
                    {
                        mr.ID = 0;
                        mr.Headers?.ForEach(h => h.ID = 0);
                    });
                }
            }
            
            fullDatabase.Tenants = tenants.Select(t =>
            {
                t.ID = 0;
                return _tenantMapper.ToTenantDto(t);
            }).ToList();
            
            return fullDatabase;
        }

        public async Task<bool> ImportDatabase(FullDatabaseDto import, bool skipDuplicateTenants)
        {
            if (import.Tenants == null)
                return false;
            var tenants = import.Tenants.Select(t=> _tenantMapper.ToTenantEntity(t));

            var existingTenant = _context.Tenants;

            foreach (var tenant in tenants)
            {
                if (!existingTenant.Any(et => et.Name.ToUpper().Equals(tenant.Name.ToUpper()) ||
                                                    et.Path.ToUpper().Equals(tenant.Path.ToUpper())
                                        ))
                {
                    _context.Tenants.Add(tenant);
                }
                else if(!skipDuplicateTenants)
                {
                    throw new InvalidOperationException("Cannot import duplicate tenant");
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
