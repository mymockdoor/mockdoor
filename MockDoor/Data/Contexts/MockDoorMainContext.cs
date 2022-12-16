using Microsoft.EntityFrameworkCore;
using MockDoor.Data.Models;
using MockDoor.Data.Models.Headers;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MockDoor.Data.Contexts
{
    public class MockDoorMainContext : DbContext
    {
        public MockDoorMainContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<ServiceGroup> ServiceGroups { get; set; }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        public DbSet<MockResponse> MockResponses { get; set; }
        
        public DbSet<Microservice> Microservices { get; set; }
        
        public DbSet<QueryParameter> QueryParameters { get; set; }

        public DbSet<ServiceHeader> ServiceHeaders { get; set; }
        
        public DbSet<RequestHeader> RequestHeaders { get; set; }
        
        public DbSet<ResponseHeader> ResponseHeaders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>()
                .HasIndex(rt => rt.Path)
                .IsUnique();
        }
    }
}
