using System;

namespace MockDoor.Shared.Models.Timetravel
{
    public class UpdateTimeTravelDto
    {
        public DateTime? Time { get; set; }

        public TimeTravelScope Scope { get; set; }
    }
}

namespace MockDoor.Shared
{
    public enum TimeTravelScope
    {
        Tenant,
        ServiceGroup,
        Microservice,
        Request
    }
}