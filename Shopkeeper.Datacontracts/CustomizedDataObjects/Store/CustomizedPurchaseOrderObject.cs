
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class PurchaseOrderObject
    {
        public string GeneratedByEmployeeNo { get; set; }
        public string GeneratedByEmployeeName { get; set; }
        public string OutletName { get; set; }
        public string SupplierName { get; set; }
        public string DeliveryStatus { get; set; }
        public string AccountCode { get; set; }
        public string AccountGroupName { get; set; }
        public string DateTimePlacedStr { get; set; }
        public string DerivedTotalCostStr { get; set; }
        public string ActualDeliveryDateStr { get; set; }
        public string ExpectedDeliveryDateStr { get; set; }

    }
}

