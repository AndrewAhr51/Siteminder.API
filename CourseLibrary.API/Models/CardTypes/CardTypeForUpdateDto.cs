using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class CardTypeForUpdateDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string IsPrivateAccount { get; set; }
    }
}
