
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class ReturnedProductObject
    {
        public long ReturnedProductId { get; set; }
        public int IssueTypeId { get; set; }
        public long StoreItemStockId { get; set; }
        public long RefundNoteId { get; set; }
        public System.DateTime DateReturned { get; set; }
        public double QuantityBought { get; set; }
        public double QuantityReturned { get; set; }
        public double AmountRefunded { get; set; }
        public double Rate { get; set; }

        public virtual IssueTypeObject IssueTypeObject { get; set; }
        public virtual RefundNoteObject RefundNoteObject { get; set; }
        public virtual StoreItemStockObject StoreItemStockObject { get; set; }
    }

    public partial class ReturnedProductObject
    {
        public string Reason { get; set; }
        public string ImagePath { get; set; }
        public string StoreItemName { get; set; }
        public string Sku { get; set; }
        public long PurchaseOrderItemId { get; set; }
        public virtual List<ItemPriceObject> ItemPriceObjects { get; set; }
    }
}
