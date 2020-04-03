using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Discount
    {
        [Key]
        public Guid Id { get; set; }
        public int Gallons { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }
        [ForeignKey("SiteId")]
        public Site Site { get; set; }
    }
}
