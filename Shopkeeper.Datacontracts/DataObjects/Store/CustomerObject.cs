
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class CustomerObject
    {
        public long CustomerId { get; set; }
        public Nullable<long> ReferredByCustomerId { get; set; }
        public int StoreCustomerTypeId { get; set; }
        public Nullable<int> StoreOutletId { get; set; }
        public long UserId { get; set; }
        public Nullable<System.DateTime> FirstPurchaseDate { get; set; }
        public Nullable<long> ContactPersonId { get; set; }
        public Nullable<System.DateTime> DateProfiled { get; set; }

        public virtual ICollection<BankAccountObject> BankAccountObjects { get; set; }
        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual UserProfileObject UserProfileObject { get; set; }
        public virtual StoreCustomerTypeObject StoreCustomerTypeObject { get; set; }
        public virtual StoreOutletObject StoreOutletObject { get; set; }
        public virtual ICollection<CustomerInvoiceObject> CustomerInvoiceObjects { get; set; }
        public virtual ICollection<DeliveryAddressObject> DeliveryAddressObjects { get; set; }
        public virtual ICollection<EstimateObject> EstimateObjects { get; set; }
        public virtual ICollection<SaleObject> SaleObjects { get; set; }
    }
}





