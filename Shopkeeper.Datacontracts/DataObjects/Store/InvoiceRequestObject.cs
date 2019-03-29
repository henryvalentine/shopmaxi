
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class InvoiceRequestObject
    {
        public long Id { get; set; }
        public long SaleId { get; set; }
        public long RequestedById { get; set; }
        public System.DateTime DateRequested { get; set; }

        public virtual SaleObject SaleObject { get; set; }
        public virtual UserProfileObject UserProfileObject { get; set; }
        public virtual ICollection<InvoiceRequestItemObject> InvoiceRequestItemObjects { get; set; }
    }
}
