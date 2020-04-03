using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class CompanyContacts
    {
        [Key]
        public Guid CompanyId { get; set; }

        [Key]
        [ForeignKey("ContactId")]
        public Guid ContactId { get; set; }
    }
}
