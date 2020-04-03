using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class TerminalContacts
    {
        [Key]
        public Guid TerminalId { get; set; }
        [Key]
        public Guid ContactId { get; set; }
    }
}
