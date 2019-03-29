
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    public partial class StoreItemSupplierObject
    {
        public long SupplierId { get; set; }
        public long StoreItemId { get; set; }
        public int MinSupplyQuantity { get; set; }
        public int MaxSupplyQuantity { get; set; }
        public Nullable<int> UnitOfMeasurement { get; set; }
        public Nullable<double> Rating { get; set; }

        public virtual StoreItemObject StoreItemObject { get; set; }
    }
}
