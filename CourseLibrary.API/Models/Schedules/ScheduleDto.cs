using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Siteminder.API.Models.ScheduleForCreationDto;

namespace Siteminder.API.Models
{
    public class ScheduleDto
    {
        public Guid ScheduleId { get; set; }

        public string ScheduleName { get; set; }

        public ScheduleDataDto[] ScheduleInfo { get; set; }

    }
   
}
