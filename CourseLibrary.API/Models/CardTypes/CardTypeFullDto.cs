using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class CardTypeFullDto
    {
        public string Id { get;  }

        public string Name { get; set; }

        public string Description { get; set; }

        public string IsPrivateAccount { get; set; }
    }
}
