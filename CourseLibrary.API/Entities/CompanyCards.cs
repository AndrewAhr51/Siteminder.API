using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class CompanyCards
    {
        [Key]
        public Guid CompanyId { get; set; }
        [Key]
        public Guid CardId { get; set; }
    }
}
