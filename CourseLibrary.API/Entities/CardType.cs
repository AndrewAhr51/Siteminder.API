using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class CardType
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Please enter a card name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter a card description")]
        [MaxLength(1500)]
        public string Description { get; set; }

        public bool IsPrivateAccount { get; set; } = false;
    }
}
