
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public class DailySaleObject
    {
        public long DailySaleId { get; set; }
        public System.DateTime SalesDate { get; set; }
        public long StoreOutletId { get; set; }
        public double AmountSold { get; set; }
        public byte[] LastUpdated { get; set; }

        public virtual StoreOutletObject StoreOutletObject { get; set; }
    }
}
