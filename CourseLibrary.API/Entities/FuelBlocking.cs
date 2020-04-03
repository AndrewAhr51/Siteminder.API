using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class FuelBlocking
    {
        [Key]
        public Guid FuelId { get; set; }

        [Key]
        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }
    }
}
