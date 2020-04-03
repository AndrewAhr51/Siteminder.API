using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Dispenser
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter a Dispenser name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a Dispenser Id")]
        [MaxLength(2)]
        public int DispenserId { get; set; }

        [Required(ErrorMessage = "Please select a Dispenser type")]
        public string DispenserType { get; set; }

        [Required(ErrorMessage = "Please select a Pulser type")]
        public string PulserType { get; set; }

        [Required(ErrorMessage = "Please enter a HV Display Number")]
        public int HVDisplayNumber { get; set; }

        [Required(ErrorMessage = "Please enter a current Totalizer Reading")]
        public int TotalizerReading { get; set; }

        [Required(ErrorMessage = "Please the maximum Totalizer digits. Ex: 9")] 
        public int MaxTotalizerDigits { get; set;} = 9;

        [Required(ErrorMessage = "Please select the volume unit")]
        public Guid  VolumeUnit { get; set; }

        [Required(ErrorMessage = "Please select the Terminal")]
        [ForeignKey("TerminalId")]
        public Guid TerminalId { get; set; }

        [ForeignKey("FuelId")]
        public Guid FuelId { get; set; }

        [ForeignKey("ScheduleId")]
        public Guid ScheduleId { get; set; }

        public ICollection<Tank> Tanks { get; set; }
        = new List<Tank>();
        public ICollection<DispenserType> DispenserTypes { get; set; }
        = new List<DispenserType>();
    }
}
