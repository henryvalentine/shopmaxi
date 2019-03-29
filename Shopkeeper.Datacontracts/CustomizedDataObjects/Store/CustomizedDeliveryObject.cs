
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class DeliveryObject
    {
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string DeliveryTypeCode { get; set; }
        public string RegistrationCode { get; set; }
        public string AcutalDeliveryDateStr { get; set; }
        public List<PurchaseOrderItemDeliveryObject> PurchaseOrderItemDeliveryObjects { get; set; }
    }
}

