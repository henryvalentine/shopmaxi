
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial  class PurchaseOrderObject
    {
        public long PurchaseOrderId { get; set; }
        public int StoreOutletId { get; set; }
        public int AccountId { get; set; }
        public long SupplierId { get; set; }
        public int StatusCode { get; set; }
        public System.DateTime DateTimePlaced { get; set; }
        public Nullable<double> DerivedTotalCost { get; set; }
        public long GeneratedById { get; set; }
        public System.DateTime ExpectedDeliveryDate { get; set; }
        public Nullable<System.DateTime> ActualDeliveryDate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public double VATAmount { get; set; }
        public double DiscountAmount { get; set; }   
        public double FOB { get; set; }
       
        public virtual ChartOfAccountObject ChartOfAccountObject { get; set; }
        public virtual ICollection<DeliveryObject> DeliveryObjects { get; set; }
        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual ICollection<InvoiceObject> InvoiceObjects { get; set; }
        public virtual StoreOutletObject StoreOutletObject { get; set; }
        public virtual SupplierObject SupplierObject { get; set; }
        public virtual ICollection<PurchaseOrderItemObject> PurchaseOrderItemObjects { get; set; }
        public virtual ICollection<PurchaseOrderPaymentObject> PurchaseOrderPaymentObjects { get; set; }
    }
}





 
  
 
 
 
 
