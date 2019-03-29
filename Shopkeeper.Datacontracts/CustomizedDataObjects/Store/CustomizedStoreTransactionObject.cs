
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreTransactionObject
    {
        public string StoreTransactionTypeName { get; set; }
        public string PaymentMethodName { get; set; }
        public string TransactionEmployeeName { get; set; }
        public string OutletName { get; set; }
        public string TransactionDateStr { get; set; }
        public string TransactionAmountStr { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public long EstimateId { get; set; }
        public double AmountRefunded { get; set; }
        public long SaleId { get; set; }
        
    }
}
