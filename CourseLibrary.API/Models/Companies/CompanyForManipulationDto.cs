using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class CompanyForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out the Name")]
        [MaxLength(50, ErrorMessage = "The Company name should have no more than 50 characters.")]
        public virtual string Name { get; set; }

        public virtual string Type { get; set; }

        [Required(ErrorMessage = "You should fill out the Address")]
        [MaxLength(50, ErrorMessage = "The Address line 1 should have no more than 50 characters.")]
        public virtual string Address1 { get; set; }
        
        [MaxLength(50, ErrorMessage = "The Address line 2 should have no more than 50 characters.")]
        public virtual string Address2 { get; set; }

        [Required(ErrorMessage = "You should fill out the City")]
        [MaxLength(30, ErrorMessage = "The City name should have no more than 50 characters.")]
        public virtual string City { get; set; }

        [Required(ErrorMessage = "You should fill out the State")]
        [MaxLength(2, ErrorMessage = "The State name should have no more than 2 characters.")]
        public virtual string State { get; set; }

        [Required(ErrorMessage = "You should fill out the Zipcode")]
        [MaxLength(9, ErrorMessage = "The Zipcode name should have no more than 5 characters;plus zip +4.")]
        public virtual string Zipcode { get; set; }

    }

}
