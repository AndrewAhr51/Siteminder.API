using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class AuthorizedCard
    {
        [Key]
        public Guid Id { get; set; }

        public string Type { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HoldAmount { get; set; } = 0;

        public string Description { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}
