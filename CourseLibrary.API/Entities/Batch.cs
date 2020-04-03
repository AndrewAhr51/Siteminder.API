using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Batch
    {
        [Key]
        public Guid Id { get; set; }

        public int BatchNumber { get; set; }

        public string Status { get; set; } = "In Process";

        public string LastAccountProcessed { get; set; } = string.Empty;

        public DateTime BatchDate { get; set; }
        
    }
}
