using Microsoft.EntityFrameworkCore;
using MockDoor.Abstractions.Repositories;
using MockDoor.Data.Contexts;
using MockDoor.Data.Models;
using MockDoor.Shared.Models.General;
using MockDoor.Shared.Models.ServiceGroup;
using MockDoor.Shared.Models.Tenant;

namespace MockDoor.Data.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly MockDoorMainContext _context;

        public TenantRepository(MockDoorMainContext context)
        {
            _context = context;
        }

        public async Task<List<PathNameItem>> GetAllTakenTenantNameAndPathsAsync()
        {
            var tenants = _context.Tenants.Select(rt => new PathNameItem(rt.Name, rt.Path));

            return await tenants.ToListAsync();
        }
        
        public async Task<List<PathNameItem>> GetAllTakenTenantNameAndPathsAsync(int excludingId)
        {
            var tenants = _context.Tenants.Where(rt => rt.ID != excludingId).Select(rt => new PathNameItem(rt.Name, rt.Path));

            return await tenants.ToListAsync();
        }

        public async Task<TenantListDto> GetAllTenantsListAsync(int skip, int take)
        {
            var tenants = await _context.Tenants.Include(t => t.ServiceGroups).ToListAsync();

            return new TenantListDto()
            {
                TotalTenants = tenants.Count,
                Tenants = tenants.Select(t =>
                {
                    return new BaseTenantDto()
                    {
                        Id = t.ID,
                        Name = t.Name,
                        Path = t.Path,
                        SimulateTime = t.SimulateTime,
                        RegisteredServiceGroups = t.ServiceGroups?.Select(rs =>
                                    new BaseServiceGroupDto()
                                    {
                                        Id = rs.ID,
                                        Name = rs.Name,
                                        DefaultHealthCheckUrl = rs.DefaultHealthCheckUrl,
                                        Enabled = rs.Enabled,
                                        Path = rs.Path,
                                        SimulateTime = rs.SimulateTime
                                    }
                        ).ToList() ?? new List<BaseServiceGroupDto>()
                    };
                }).Skip(skip).Take(take).ToList()
            };
        }

        public async Task<BaseTenantDto> GetTenantByIdAsync(int id)
        {
            var tenant = await _context.Tenants.Include(t => t.ServiceGroups).FirstOrDefaultAsync(t => t.ID == id);

            return tenant == null ? null : new BaseTenantDto()
            {
                Id = tenant.ID,
                Name = tenant.Name,
                Path = tenant.Path,
                SimulateTime = tenant.SimulateTime,
                RegisteredServiceGroups = tenant.ServiceGroups?.Select(rs =>
                            new BaseServiceGroupDto()
                            {
                                Id = rs.ID,
                                Name = rs.Name,
                                DefaultHealthCheckUrl = rs.DefaultHealthCheckUrl,
                                Enabled = rs.Enabled,
                                Path = rs.Path,
                                SimulateTime = rs.SimulateTime
                            }
                        ).ToList() ?? new List<BaseServiceGroupDto>()
            };
        }

        public async Task<BaseTenantDto> GetTenantByNameAsync(string name)
        {
            var tenant = await _context.Tenants.Include(t => t.ServiceGroups).FirstOrDefaultAsync(t => t.Name == name);


            return tenant == null ? null : new BaseTenantDto()
            {
                Id = tenant.ID,
                Name = tenant.Name,
                Path = tenant.Path,
                SimulateTime = tenant.SimulateTime,
                RegisteredServiceGroups = tenant.ServiceGroups?.Select(rs =>
                            new BaseServiceGroupDto()
                            {
                                Id = rs.ID,
                                Name = rs.Name,
                                DefaultHealthCheckUrl = rs.DefaultHealthCheckUrl,
                                Enabled = rs.Enabled,
                                Path = rs.Path,
                                SimulateTime = rs.SimulateTime
                            }
                        ).ToList() ?? new List<BaseServiceGroupDto>()
            };
        }

        public async Task<BaseTenantDto> GetTenantByPathAsync(string path)
        {
            var tenant = await _context.Tenants.Include(t => t.ServiceGroups).FirstOrDefaultAsync(t => t.Path == path.ToLower());

            return tenant == null ? null : new BaseTenantDto()
            {
                Id = tenant.ID,
                Name = tenant.Name,
                Path = tenant.Path,
                SimulateTime = tenant.SimulateTime,
                RegisteredServiceGroups = tenant.ServiceGroups?.Select(rs =>
                            new BaseServiceGroupDto()
                            {
                                Id = rs.ID,
                                Name = rs.Name,
                                DefaultHealthCheckUrl = rs.DefaultHealthCheckUrl,
                                Enabled = rs.Enabled,
                                Path = rs.Path,
                                SimulateTime = rs.SimulateTime
                            }
                        ).ToList() ?? new List<BaseServiceGroupDto>()
            };
        }

        public async Task<BaseTenantDto> CreateTenantAsync(BaseTenantDto newTenantDto)
        {
            if (newTenantDto == null)
                throw new Exception("No tenant provided");

            if (string.IsNullOrWhiteSpace(newTenantDto.Path))
                throw new Exception("Error path missing or empty");

            var existingTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Path.ToLower() == newTenantDto.Path.ToLower());

            if (existingTenant != null)
                throw new Exception("tenant with same path already exists. Tenant paths MUST be unique");

            var newTenant = new Tenant()
            {
                Name = newTenantDto.Name,
                Path = newTenantDto.Path.ToLower(),
                SimulateTime = newTenantDto.SimulateTime
            };

            _context.Tenants.Add(newTenant);

            await _context.SaveChangesAsync();

            return new BaseTenantDto()
            {
                Id = newTenant.ID,
                Name = newTenant.Name,
                Path = newTenant.Path,
                SimulateTime = newTenant.SimulateTime
            };
        }

        /// <summary>
        /// Update the base properties ONLY on a tenant
        /// </summary>
        /// <param name="updatedTenant">the updated tenant</param>
        /// <returns>true if updated successfully</returns>
        public async Task<bool> UpdateTenantBaseValuesAsync(BaseTenantDto updatedTenant)
        {
            var existingTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.ID == updatedTenant.Id);

            if (existingTenant == null)
                return false;

            existingTenant.Name = updatedTenant.Name;
            existingTenant.Path = updatedTenant.Path.ToLower();
            existingTenant.SimulateTime = updatedTenant.SimulateTime;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTenantAsync(int id)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.ID == id);

            if (tenant == null)
                return false;

            _context.Tenants.Remove(tenant);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
