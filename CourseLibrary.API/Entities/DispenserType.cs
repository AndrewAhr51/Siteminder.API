﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class DispenserType
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter a Dispenser type name")]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter a Dispenser type description")]
        [MaxLength(1500)]
        public string Description { get; set; }
    }
}
