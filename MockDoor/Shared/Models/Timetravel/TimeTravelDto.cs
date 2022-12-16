using System;
using System.Collections.Generic;

namespace MockDoor.Shared.Models.Timetravel
{
    public class TimeTravelDto
    {
        public List<DateTime> AvailableTimes { get; set; }

        public DateTime? CurrentTime { get; set; }
    }
}
