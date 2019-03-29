
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreTransactionObject
    {
        public long StoreTransactionId { get; set; }
        public int StoreTransactionTypeId { get; set; }
        public int StorePaymentMethodId { get; set; }
        public long EffectedByEmployeeId { get; set; }
        public double TransactionAmount { get; set;}
        public System.DateTime TransactionDate { get; set; }
        public string Remark { get; set; }
        public int StoreOutletId { get; set; }

        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual ICollection<PurchaseOrderPaymentObject> PurchaseOrderPaymentObjects { get; set; }
        public virtual ICollection<SaleTransactionObject> SaleTransactionObjects { get; set; }
        public virtual StoreOutletObject StoreOutletObject { get; set; }
        public virtual StorePaymentMethodObject StorePaymentMethodObject { get; set; }
        public virtual StoreTransactionTypeObject StoreTransactionTypeObject { get; set; }
    }
}
