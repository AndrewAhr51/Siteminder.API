using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class ScheduleDetail
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please select a day of the week")]
        public string DayOfWeek { get; set; }

        [Required(ErrorMessage = "Please enter an opening time")]
        public DateTime OpenTime { get; set; }

        [Required(ErrorMessage = "Please enter an closing time")]
        public DateTime CloseTime { get; set; }

        [ForeignKey("ScheduleId")]
        public Guid ScheduleId { get; set; }
    }
}
