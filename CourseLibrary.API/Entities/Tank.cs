using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Tank
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter a Tank name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a Tank Description")]
        [MaxLength(1500)]
        public int Description { get; set; }
        
        [Required(ErrorMessage = "Please enter a Tank size")]
        public int Size { get; set; }

        [Required(ErrorMessage = "Please enter a Tank level")]
        public int Level { get; set; }

        [Required(ErrorMessage = "Please enter a Tank Alarm point")]
        public int AlarmPoint { get; set; }

        [ForeignKey("TerminalId")]
        public Guid TerminalId { get; set; }

        [ForeignKey("DispenserId")]
        public Guid DispenserId { get; set; }

        [ForeignKey("FuelTypeId")]
        public Guid FuelTypeId { get; set; }
        public ICollection<VolumeUnits> VolumeUnits { get; set; }
        = new List<VolumeUnits>();
    }
}
