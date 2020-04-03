using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Schedule
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter an Schedule name")]
        public string ScheduleName { get; set; }

        [ForeignKey("TerminalId")]
        public Guid TerminalId { get; set; }

        public ICollection<ScheduleDetail> ScheduleDetails { get; set; }
        = new List<ScheduleDetail>();

    }

}


