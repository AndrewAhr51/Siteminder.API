using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class TerminalDispenserAssociation
    {

        [Key]
        [ForeignKey("DispenserId")]
        public Guid DispenserId { get; set; }
        [Key]
        [ForeignKey("TerminalId")]
        public Guid TerminalId { get; set; }
    }
}
