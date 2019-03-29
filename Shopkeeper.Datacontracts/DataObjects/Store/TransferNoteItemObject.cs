

using System;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class TransferNoteItemObject
    {
        public long Id { get; set; }
        public long TransferNoteId { get; set; }
        public double TotalQuantityRaised { get; set; }
        public double TotalQuantityTransfered { get; set; }
        public double TotalAmountRaised { get; set; }
        public double TotalAmountTransfered { get; set; }
        public double Rate { get; set; }
        public long StoreItemStockId { get; set; }
        public DateTime? DateTransferd { get; set; }
        public long UoMId { get; set; }

        public virtual StoreItemStockObject StoreItemStockObject { get; set; }
        public virtual TransferNoteObject TransferNoteObject { get; set; }
        public virtual UnitsOfMeasurementObject UnitsOfMeasurementObject { get; set; }
    }


    public partial class TransferNoteItemObject
    {
        public string SKU { get; set; }
        public string StoreItemName { get; set; }
        public string ImagePath { get; set; }
        public string UoMCode { get; set; }
        public double BaseSellingPrice { get; set; }
        public string TransferStatus { get; set; }
    }
}
