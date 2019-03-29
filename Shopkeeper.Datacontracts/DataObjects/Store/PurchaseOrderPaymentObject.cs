
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    public partial class PurchaseOrderPaymentObject
    {
        public long PurchaseOrderPaymentId { get; set; }
        public long StoreTransactionId { get; set; }
        public long PurchaseOrderId { get; set; }
        public double? AmountPaid { get; set; }
        public DateTime DateMade { get; set; }
        public string InvoiceFilePath { get; set; }
        public string Remark { get; set; }

        public virtual PurchaseOrderObject PurchaseOrderObject { get; set; }
        public virtual StoreTransactionObject StoreTransactionObject { get; set; }
    }
}
