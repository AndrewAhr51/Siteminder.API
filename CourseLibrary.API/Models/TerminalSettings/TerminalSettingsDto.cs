using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class TerminalSettingsDto
    {
        public string Id { get; set; }
        public string TerminalId { get; set; }
        public string ScheduleId { get; set; }
        public string PrintPvtAccountBalanceOnReceipt { get; set; }
        public string BatchCloseTime { get; set; }
        public string TimeZoneId { get; set; }
        public string CCPlatform { get; set; }
        public string RejectUnregisteredNonPrivateCardUsers { get; set; }
        public string RejectUnregisteredPrivateCardUsers { get; set; }
        public string CummulativeRiskAmount { get; set; }
        public string MaxiumumOfflineAmount { get; set; }
        public string OfflineSalesOption { get; set; }
        public string AmountOfFillupKey { get; set; }
        public string MaximumSaleAmount { get; set; }
        public string TaxTableId { get; set; }
        public string DisableFillupKey { get; set; }
        public string ProprietaryCardCode { get; set; }
        public string QTGatewayTerminalId { get; set; }
        public string MSATerminalId { get; set; }
        public string MSAUserName { get; set; }
        public string MSTSMerchantAccountID { get; set; }
        public string HeartlandCompanyId { get; set; }
        public string HeartlandDeviceId { get; set; }
        public string TargetNetworkEnvironment { get; set; }
        public string QTGatewayAccessCode { get; set; }
        public string MSAMechantNumber { get; set; }
        public string MSAPassword { get; set; }
        public string MSTSMerchantJobberId { get; set; }
        public string HeartlandTerminalLocationId { get; set; }
        public string DPIPort { get; set; }
        public string PIEPort { get; set; }
        public string HVDPOrt { get; set; }
        public string BarcodeReaderPort { get; set; }
        public string AutoTankGuaging { get; set; }
        public string ATGPort { get; set; }
        public string ATGBaudRate { get; set; }
        public string ATGParity { get; set; }
        public string ATGStopBits { get; set; }
        public string ATGDataBits { get; set; }
        public string ATGHandshake { get; set; }
        public string ATGSecurityCode { get; set; }
        public string ForceRegistry { get; set; }
        public string RegistryMinLength { get; set; } 
        public string RegistryMaxLength { get; set; } 
        public string RegistryLabelAs { get; set; }
    }
}
