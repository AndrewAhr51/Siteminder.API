using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Terminal
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Please enter a Terminal name")]
        [MaxLength(50)]
        public string TerminalName { get; set; }

        [ForeignKey("SiteId")]
        public Guid SiteId { get; set; }

        public ICollection<Schedule> Schedules { get; set; }
        = new List<Schedule>();
        public ICollection<FuelPump> FuelPumps { get; set; }
          = new List<FuelPump>();
        public ICollection<TerminalSettings> TerminalSettings { get; set; }
          = new List<TerminalSettings>();
        public ICollection<Dispenser> Dispensers { get; set; }
          = new List<Dispenser>();


    }
}
