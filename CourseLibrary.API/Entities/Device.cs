using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Device
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter a Device name")]
        [MaxLength(50)]
        public string DeviceName { get; set; }

        [Required(ErrorMessage = "Please enter a Model name")]
        [MaxLength(50)]
        public string ModelName { get; set; }

        [Required(ErrorMessage = "Please enter a serial number name")]
        [MaxLength(50)]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Please enter a description")]
        [MaxLength(1500)]
        public string Description { get; set; }

        [ForeignKey("TerminalId")]
        public Guid TerminalId { get; set; }
    }
}
