﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class FuelFullDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid FuelTypeId { get; set; }
    }
}
