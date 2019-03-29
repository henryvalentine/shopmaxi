
using System;
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreItemSoldObject
    {
        public long StoreItemSoldId { get; set; }
        public long StoreItemStockId { get; set; }
        public long SaleId { get; set; }
        public double QuantitySold { get; set; }
        public double AmountSold { get; set; }
        public int UoMId { get; set; }
        public System.DateTime DateSold { get; set; }
        public double QuantityDelivered { get; set; }
        public double QuantityBalance { get; set; }
        public double Rate { get; set; }
        public double QuantityReturned { get; set; }

        public virtual List<InvoiceRequestItemObject> InvoiceRequestItemObjects { get; set; }
        public virtual SaleObject SaleObject { get; set; }
        public virtual StoreItemStockObject StoreItemStockObject { get; set; }
    }


}
