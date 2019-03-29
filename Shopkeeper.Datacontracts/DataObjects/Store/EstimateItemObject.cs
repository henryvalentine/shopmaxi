
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class EstimateItemObject
    {
        public long Id { get; set; }
        public long StoreItemStockId { get; set; }
        public double QuantityRequested { get; set; }
        public double ItemPrice { get; set; }
        public long EstimateId { get; set; }

        public virtual EstimateObject EstimateObject { get; set; }
        public virtual StoreItemStockObject StoreItemStockObject { get; set; }

    }

      public partial class EstimateItemObject
      {
          public string SerialNumber { get; set; }
          public string StoreItemName { get; set; }
          public string UoMCode { get; set; }
          public string ImagePath { get; set; }
    }
   
    
}

