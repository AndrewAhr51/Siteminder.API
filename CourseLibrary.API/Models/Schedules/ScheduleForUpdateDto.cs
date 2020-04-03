using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class ScheduleForUpdateDto
    { 
        public string ScheduleName { get; set; }

        public ScheduleDataDto[] ScheduleInfo { get; set; }

    }
}
