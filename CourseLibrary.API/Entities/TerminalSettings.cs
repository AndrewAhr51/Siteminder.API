using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class TerminalSettings
    {
        [Key]
        public Guid Id { get; set; }
        public bool PrintPvtAccountBalanceOnReceipt { get; set; }
        public DateTime BatchCloseTime { get; set; } = DateTime.Today.AddDays(1);
        public string TimeZoneId { get; set; } = "MST";
        public string CCPlatform { get; set; } = "Phillips66";
        public bool RejectUnregisteredNonPrivateCardUsers { get; set; } = false;
        public bool RejectUnregisteredPrivateCardUsers { get; set; } = false;
        [Column(TypeName = "decimal(18,2)")]
        public decimal CummulativeRiskAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxiumumOfflineAmount { get; set; }
        public bool OfflineSalesOption { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountOfFillupKey { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaximumSaleAmount { get; set; }
        public string TaxTableId { get; set; }
        public bool DisableFillupKey { get; set; } = false;
        public int ProprietaryCardCode { get; set; }
        public string QTGatewayTerminalId { get; set; }
        public string MSATerminalId { get; set; }
        public string MSAUserName { get; set; }
        public string MSTSMerchantAccountID { get; set; }
        public string HeartlandCompanyId { get; set; }
        public string HeartlandDeviceId { get; set; }
        public string TargetNetworkEnvironment { get; set; }
        public string QTGatewayAccessCode { get; set; }
        public string MSAMechantNumber{ get; set; }
        public string MSAPassword { get; set; }
        public string MSTSMerchantJobberId { get; set; }
        public string HeartlandTerminalLocationId { get; set; }
        public string DPIPort { get; set; }
        public string PIEPort { get; set; }
        public string HVDPOrt { get; set; }
        public string BarcodeReaderPort { get; set; }
        public string AutoTankGuaging { get; set; }
        public  string ATGPort { get; set; }
        public int ATGBaudRate { get; set; }
        public string ATGParity { get; set; }
        public string ATGStopBits { get; set; }
        public string ATGDataBits { get; set; }
        public string ATGHandshake { get; set; }
        public string ATGSecurityCode { get; set; }
        public bool ForceRegistry { get; set; } 
        public int RegistryMinLength { get; set; } 
        public int RegistryMaxLength { get; set; }
        public string RegistryLabelAs { get; set; }


        [ForeignKey("TerminalId")]
        public Guid TerminalId { get; set; }

        [ForeignKey("ScheduleId")]
        public Guid ScheduleId { get; set; }
    }
}
