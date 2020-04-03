using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Transaction
    {
        [Key]
        public string Id { get; set; }
        [Required(ErrorMessage ="Transaction date is missing")]
        public DateTime TransactionDate { get; set; }
        public string SortableDate { get; set; }
        public string BatchNumber { get; set; }
        [Required(ErrorMessage = "Approval Code is missing")]
        public string ApprovalCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AuthorizedAmount { get; set; }
        public string TerminalName { get; set; }
        public string AuthResponseData { get; set; }
        public string CustomTicketMessage { get; set; }
        public string IssuerResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseText { get; set; }
        public string CaptureResponseMessage { get; set; }
        public string SettlementResponseText { get; set; }
        public DateTime ResponseTimestamp { get; set; }
        public int TraceNumber { get; set; }
        public string ApprovingNetworkName { get; set; }
        public string AuthorizationTimeout { get; set; }
        public string CardType { get; set; }
        public string ComdataID { get; set; }
        public string DriverID { get; set; }
        public string EntryMode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string NameOnCard { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaymentAmount { get; set; }
        [Required(ErrorMessage ="Zipcode is required")]
        public string Zipcode { get; set; }
        public string VehicleID { get; set; }
        public string NetworkType { get; set; }
        public string Processor { get; set; }
        public string SettlementResponseCode { get; set; }
        public string SettlementResponseID { get; set; }
        public string CancelResponseMessage { get; set; }
        public string CancelResponseText { get; set; }
        public string InvoiceNumber { get; set; }
        public string FlightNumber { get; set; }
        public string TailNumber { get; set; }
        public int Status { get; set; } = 0;
        public string StartTotalizer { get; set; }
        public string EndTotalizer { get; set; }
        public string DispenserID { get; set; }
        public string DispenserName { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }
        public string TotalSaleAmount { get; set; }
        public string AvgPricePerUnit { get; set; }
        public string VolumeUnitName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalTax { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal VolumeGross { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal VolumeNet { get; set; }
        public string Temperature { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string ReceiptAvailable { get; set; }
        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }
        
    }
}
