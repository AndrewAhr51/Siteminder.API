using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class FuelPump
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter a Fuel Pump name")]
        [MaxLength(50)]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Please enter a Fuel Pump Description")]
        [MaxLength(1500)]
        public string Description { get; set; }
        [ForeignKey("TerminalId")]
        public Terminal Terminal { get; set; }

        [ForeignKey("FuelId")]
        public Fuel Fuel { get; set; }

        public ICollection<FuelType> FuelTypes { get; set; }
         = new List<FuelType>();
    }
}
