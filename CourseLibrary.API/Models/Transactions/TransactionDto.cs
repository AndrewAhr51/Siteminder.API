using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class TransactionDto
    {
        public DateTime TransactionDate { get; set; }
        public string SortableDate { get; set; }
        public string BatchNumber { get; set; }
        public string ApprovalCode { get; set; }
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
        public string AccountNumber { get; set; }
        public string CardType { get; set; }
        public string ComdataID { get; set; }
        public string DriverID { get; set; }
        public string EntryMode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string NameOnCard { get; set; }
        public decimal PaymentAmount { get; set; }
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
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TotalSaleAmount { get; set; }
        public string AvgPricePerUnit { get; set; }
        public string VolumeUnitName { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalTax { get; set; }
        public decimal VolumeGross { get; set; }
        public decimal VolumeNet { get; set; }
        public string Temperature { get; set; }
        public string AccountID { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string ReceiptAvailable { get; set; }
        public Guid AccountId { get; set; }
    }
}
