using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class TankFuelAssociation
    {
        [Key]
        [ForeignKey("TankId")]
        public Guid TankId { get; set; }
        [Key]
        [ForeignKey("FuelId")]
        public Guid FuelId { get; set; }
       
    }
}
