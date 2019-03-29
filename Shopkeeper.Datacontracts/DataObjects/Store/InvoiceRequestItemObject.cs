
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class InvoiceRequestItemObject
    {
        public long Id { get; set; }
        public long StoreItemSoldId { get; set; }
        public long InvoiceRequestId { get; set; }
        public double QuantityDispatched { get; set; }
        public System.DateTime DateDispatched { get; set; }

        public virtual InvoiceRequestObject InvoiceRequestObject { get; set; }
        public virtual StoreItemSoldObject StoreItemSoldObject { get; set; }
    }
}
