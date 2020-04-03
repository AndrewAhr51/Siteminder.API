using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter a Role Name")]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
