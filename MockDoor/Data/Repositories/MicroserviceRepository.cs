﻿using Microsoft.EntityFrameworkCore;
using MockDoor.Abstractions.Repositories;
using MockDoor.Data.Contexts;
using MockDoor.Data.Mappers;
using MockDoor.Data.Models;
using MockDoor.Shared.Models.General;
using MockDoor.Shared.Models.Microservice;
using MockDoor.Shared.Models.Tenant;
using MockDoor.Shared.Models.Utility;

namespace MockDoor.Data.Repositories
{
    public class MicroserviceRepository : IMicroserviceRepository
    {
        private readonly MockDoorMainContext _context;

        public MicroserviceRepository(MockDoorMainContext context)
        {
            _context = context;
        }

        public async Task<List<PathNameItem>> GetAllMicroservicePathAndNamesForServiceGroup(int serviceGroupId)
        {
            var paths = _context.Microservices.Where(pd => pd.ServiceGroupID == serviceGroupId)
                                                                                    .Select(rs => new PathNameItem(rs.Name, rs.Path));

            return await paths.ToListAsync();
        }

        public async Task<List<PathNameItem>> GetAllMicroservicePathAndNamesForServiceGroup(int serviceGroupId, int excludingMicroserviceId)
        {
            var paths = _context.Microservices.Where(pd => pd.ServiceGroupID == serviceGroupId && pd.ID != excludingMicroserviceId).Select(rs => new PathNameItem(rs.Name, rs.Path));

            return await paths.ToListAsync();
        }

        public async Task<int?> GetMicroservice(int serviceGroupId, string microservicePath)
        {
            var microserviceId = await _context.Microservices.FirstOrDefaultAsync(ms => ms.ServiceGroupID == serviceGroupId && ms.Path == microservicePath);

            return microserviceId?.ID;
        }

        public async Task<MicroserviceResultDto> GetMicroserviceById(int id)
        {
            var ms = await _context.Microservices.Include(m => m.Headers).FirstOrDefaultAsync(ms => ms.ID == id);

            if (ms == null)
                return null;

            return new MicroserviceResultDto()
            {
                Id = ms.ID,
                Name = ms.Name,
                Path = ms.Path,
                Enabled = ms.Enabled,
                PassThroughTenant = ms.PassThroughTenant,
                FakeDelay = ms.FakeDelay,
                RegisteredServiceGroupId = ms.ServiceGroupID,
                TargetUrl = ms.TargetUrl,
                ProxyMode = ms.ProxyMode,
                RandomiseMockResult = ms.RandomiseMockResult,
                HeadersMode = ms.HeadersMode,
                SimulateTime = ms.SimulateTime,
                Headers = ms.Headers.ToDtos()
            };
        }

        public async Task<IEnumerable<MicroserviceResultDto>> GetAllMicroserviceForTenant(int tenantId)
        {
            var microservices = await _context.Microservices.Include(p => p.ServiceGroup).Where(pd => pd.ServiceGroup != null && pd.ServiceGroup.TenantID == tenantId).ToListAsync();

            return microservices.Select(ms => new MicroserviceResultDto()
            {
                Id = ms.ID,
                Name = ms.Name,
                Path = ms.Path,
                Enabled = ms.Enabled,
                PassThroughTenant = ms.PassThroughTenant,
                FakeDelay = ms.FakeDelay,
                RegisteredServiceGroupId = ms.ServiceGroupID,
                TargetUrl = ms.TargetUrl,
                ProxyMode = ms.ProxyMode,
                RandomiseMockResult = ms.RandomiseMockResult,
                HeadersMode = ms.HeadersMode,
                SimulateTime = ms.SimulateTime,
                Headers = ms.Headers.ToDtos()
            });
        }

        public async Task<IEnumerable<MicroserviceResultDto>> GetAllMicroservices()
        {
            var microservices = await _context.Microservices.ToListAsync();

            return microservices.Select(ms => new MicroserviceResultDto()
            {
                Id = ms.ID,
                Name = ms.Name,
                Path = ms.Path,
                Enabled = ms.Enabled,
                PassThroughTenant = ms.PassThroughTenant,
                FakeDelay = ms.FakeDelay,
                RegisteredServiceGroupId = ms.ServiceGroupID,
                TargetUrl = ms.TargetUrl,
                ProxyMode = ms.ProxyMode,
                RandomiseMockResult = ms.RandomiseMockResult,
                HeadersMode = ms.HeadersMode,
                SimulateTime = ms.SimulateTime,
                Headers = ms.Headers.ToDtos()
            });
        }

        public async Task<IEnumerable<MicroserviceSearchResultDto>> GetAllMicroserviceSearchResults()
        {
            var microservices = await _context.Microservices
                                                            .Include(m => m.ServiceRequests)
                                                            .Include(m => m.ServiceGroup)
                                                            .ThenInclude(sg => sg.Tenant)
                                                            .ToListAsync();

            return microservices.Select(ms => new MicroserviceSearchResultDto()
            {
                Id = ms.ID,
                Name = ms.Name,
                Path = ms.Path,
                Enabled = ms.Enabled,
                PassThroughTenant = ms.PassThroughTenant,
                FakeDelay = ms.FakeDelay,
                RegisteredServiceGroupId = ms.ServiceGroupID,
                TargetUrl = ms.TargetUrl,
                ProxyMode = ms.ProxyMode,
                RandomiseMockResult = ms.RandomiseMockResult,
                HeadersMode = ms.HeadersMode,
                SimulateTime = ms.SimulateTime,
                Headers = ms.Headers.ToDtos(),
                TenantId = ms.ServiceGroup.TenantID,
                TenantName = ms.ServiceGroup.Tenant.Name,
                ServiceGroupId = ms.ServiceGroupID,
                ServiceGroupName = ms.ServiceGroup.Name,
                TotalRequests = ms.ServiceRequests?.Count ?? 0
            });
            
        }

        public async Task<IEnumerable<MicroserviceResultDto>> GetAllMicroservicesForServiceGroup(int serviceId)
        {
            var microservices = await _context.Microservices.Where(pd => pd.ServiceGroupID == serviceId).ToListAsync();

            return microservices.Select(ms => new MicroserviceResultDto()
            {
                Id = ms.ID,
                Name = ms.Name,
                Path = ms.Path,
                Enabled = ms.Enabled,
                PassThroughTenant = ms.PassThroughTenant,
                FakeDelay = ms.FakeDelay,
                RegisteredServiceGroupId = ms.ServiceGroupID,
                TargetUrl = ms.TargetUrl,
                ProxyMode = ms.ProxyMode,
                RandomiseMockResult = ms.RandomiseMockResult,
                HeadersMode = ms.HeadersMode,
                SimulateTime = ms.SimulateTime,
                Headers = ms.Headers.ToDtos()
            });
        }

        public async Task<MicroserviceResultDto> FindMicroservice(string tenantPath, string serviceGroupPath, string path)
        {
            if (string.IsNullOrWhiteSpace(tenantPath) || string.IsNullOrWhiteSpace(serviceGroupPath) || string.IsNullOrWhiteSpace(path))
                return null;

            var microservice = await _context.Microservices.Include(m => m.Headers).Include(m => m.ServiceGroup).ThenInclude(rs => rs.Tenant).FirstOrDefaultAsync(m =>

                m.Path.ToLower() == path.ToLower() && m.ServiceGroup.Path.ToLower() == serviceGroupPath.ToLower() && m.ServiceGroup.Tenant.Path.ToLower() == tenantPath.ToLower()
            );

            if (microservice == null) return null;

            return new MicroserviceResultDto()
            {
                Id = microservice.ID,
                Name = microservice.Name,
                Path = microservice.Path,
                Enabled = microservice.Enabled,
                PassThroughTenant = microservice.PassThroughTenant,
                FakeDelay = microservice.FakeDelay,
                RegisteredServiceGroupId = microservice.ServiceGroupID,
                TargetUrl = microservice.TargetUrl,
                ProxyMode = microservice.ProxyMode,
                RandomiseMockResult = microservice.RandomiseMockResult,
                HeadersMode = microservice.HeadersMode,
                Headers = microservice.Headers.ToDtos(),
                SimulateTime = microservice.SimulateTime
            };
        }

        public async Task<MatchingRequestMicroserviceDetailsDto> FindMatchingRequest(string tenantPath, string serviceGroupPath, string path)
        {
            if (string.IsNullOrWhiteSpace(tenantPath) || string.IsNullOrWhiteSpace(serviceGroupPath) || string.IsNullOrWhiteSpace(path))
                return null;

            var microservice = await _context.Microservices.Include(m => m.Headers).Include(m => m.ServiceGroup).ThenInclude(rs => rs.Tenant).FirstOrDefaultAsync(m =>

                m.Path.ToLower() == path.ToLower() && m.ServiceGroup.Path.ToLower() == serviceGroupPath.ToLower() && m.ServiceGroup.Tenant.Path.ToLower() == tenantPath.ToLower()
            );

            if (microservice == null) return null;

            return new MatchingRequestMicroserviceDetailsDto()
            {
                Microservice = new MicroserviceResultDto()
                {
                    Id = microservice.ID,
                    Name = microservice.Name,
                    Path = microservice.Path,
                    Enabled = microservice.Enabled,
                    PassThroughTenant = microservice.PassThroughTenant,
                    FakeDelay = microservice.FakeDelay,
                    RegisteredServiceGroupId = microservice.ServiceGroupID,
                    TargetUrl = microservice.TargetUrl,
                    ProxyMode = microservice.ProxyMode,
                    RandomiseMockResult = microservice.RandomiseMockResult,
                    Headers = microservice.Headers.ToDtos(),
                    HeadersMode = microservice.HeadersMode,
                    SimulateTime = microservice.SimulateTime
                },
                ServiceGroup = microservice.ServiceGroup.ToBasicGroupDto(),
                Tenant = new BaseTenantDto() {
                    Id = microservice.ServiceGroup.Tenant.ID,
                    Name = microservice.ServiceGroup.Tenant.Name,
                    SimulateTime = microservice.ServiceGroup.Tenant.SimulateTime,
                    Path = microservice.ServiceGroup.Tenant.Path
                }
            };
        }

        public async Task<MicroserviceResultDto> CreateMicroservice(MicroserviceResultDto newMicroserviceDto)
        {
            var service = await _context.ServiceGroups.FirstOrDefaultAsync(t => t.ID == newMicroserviceDto.RegisteredServiceGroupId);

            if (service == null)
                return null;

            if (newMicroserviceDto == null)
                throw new Exception("Error new microservice not provided");

            if (string.IsNullOrWhiteSpace(newMicroserviceDto.Path))
                throw new Exception("Error microservice must have a path specified");

            var newMicroservice = new Microservice()
            {
                Name = newMicroserviceDto.Name,
                TargetUrl = newMicroserviceDto.TargetUrl,
                Path = newMicroserviceDto.Path.ToLower(),
                ServiceGroupID = newMicroserviceDto.RegisteredServiceGroupId,
                Enabled = newMicroserviceDto.Enabled,
                PassThroughTenant = newMicroserviceDto.PassThroughTenant,
                FakeDelay = newMicroserviceDto.FakeDelay,
                ProxyMode = newMicroserviceDto.ProxyMode,
                RandomiseMockResult = newMicroserviceDto.RandomiseMockResult,
                Headers = newMicroserviceDto.Headers.ToModels(),
                HeadersMode = newMicroserviceDto.HeadersMode,
                SimulateTime = newMicroserviceDto.SimulateTime
            };

            _context.Microservices.Add(newMicroservice);

            await _context.SaveChangesAsync();

            return new MicroserviceResultDto()
            {
                Id = newMicroservice.ID,
                Name = newMicroservice.Name,
                TargetUrl = newMicroservice.TargetUrl,
                Path = newMicroservice.Path,
                RegisteredServiceGroupId = newMicroservice.ServiceGroupID,
                Enabled = newMicroservice.Enabled,
                PassThroughTenant = newMicroservice.PassThroughTenant,
                FakeDelay = newMicroservice.FakeDelay,
                ProxyMode = newMicroservice.ProxyMode,
                RandomiseMockResult = newMicroservice.RandomiseMockResult,
                Headers = newMicroservice.Headers.ToDtos(),                
                HeadersMode = newMicroserviceDto.HeadersMode,
                SimulateTime = newMicroserviceDto.SimulateTime
            };
        }

        public async Task<bool> UpdateMicroservice(MicroserviceResultDto updatedMicroservice)
        {
            if (updatedMicroservice == null)
                return false;

            if (string.IsNullOrWhiteSpace(updatedMicroservice.Path))
                return false;

            var existingMicroservice = await _context.Microservices.Include(m => m.Headers).FirstOrDefaultAsync(t => t.ID == updatedMicroservice.Id);

            if (existingMicroservice == null)
                return false;

            existingMicroservice.Name = updatedMicroservice.Name;
            existingMicroservice.Path = updatedMicroservice.Path.ToLower();
            existingMicroservice.TargetUrl = updatedMicroservice.TargetUrl;
            existingMicroservice.Enabled = updatedMicroservice.Enabled;
            existingMicroservice.PassThroughTenant = updatedMicroservice.PassThroughTenant;
            existingMicroservice.FakeDelay = updatedMicroservice.FakeDelay;
            existingMicroservice.ProxyMode = updatedMicroservice.ProxyMode;
            existingMicroservice.RandomiseMockResult = updatedMicroservice.RandomiseMockResult;
            existingMicroservice.SimulateTime = updatedMicroservice.SimulateTime;
            existingMicroservice.HeadersMode = updatedMicroservice.HeadersMode;

            if (updatedMicroservice.Headers == null || updatedMicroservice.Headers.Count == 0)
            {
                //  clearing all headers
                existingMicroservice.Headers?.Clear();
            }
            else
            {
                // updating headers
                if (existingMicroservice.Headers == null || existingMicroservice.Headers.Count == 0)
                {
                    // Adding headers to empty list
                    existingMicroservice.Headers = updatedMicroservice.Headers.ToModels();
                }
                else
                {
                    // merging headers

                    // delete headers not in updated list
                    var headersToDelete = existingMicroservice.Headers?
                                        .Where(eh => !updatedMicroservice.Headers?.Any(uh => uh.Name == eh.Name) ?? false)
                                        .Select(eh => eh.ID).ToList();

                    if (headersToDelete != null && headersToDelete.Any())
                    {
                        existingMicroservice.Headers.RemoveAll(eh => headersToDelete.Any(htd => htd == eh.ID));
                    }

                    // add headers missing from current list
                    var headersToAdd = updatedMicroservice.Headers?
                                            .Where(uh => existingMicroservice.Headers.All(eh => eh.Name != uh.Name))
                                            .Select(h => h.ToEntity())
                                            .ToList();

                    if (headersToAdd != null && headersToAdd.Any())
                    {
                        if (existingMicroservice.Headers == null)
                        {
                            existingMicroservice.Headers = headersToAdd;
                        }
                        else
                        {
                            existingMicroservice.Headers.AddRange(headersToAdd);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMicroservice(int id)
        {
            var existingMicroservice = await _context.Microservices.FirstOrDefaultAsync(m => m.ID == id);

            if (existingMicroservice == null)
                return false;

            _context.Microservices.Remove(existingMicroservice);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MicroserviceParentIds> GetParentIds(int microserviceId)
        {
            var microservice = await _context.Microservices.Include(m => m.ServiceGroup).ThenInclude(sg => sg.Tenant).FirstOrDefaultAsync(m => m.ID == microserviceId);

            if(microservice == null)
                return null;

            return new MicroserviceParentIds() 
            {
                MicroserviceId = microservice.ID,
                ServiceGroupId = microservice.ServiceGroup.ID,
                TenantId = microservice.ServiceGroup.TenantID
            };
        }
    }
}
