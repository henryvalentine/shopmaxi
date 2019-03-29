
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class StorePaymentMethodObject
    {
        public int StorePaymentMethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<StoreTransactionObject> StoreTransactionObjects { get; set; }
    }

    public partial class StorePaymentMethodObject
    {
        public int TotalTransactions { get; set; }
        public double TotalTransactionValue { get; set; }
        public double TotalAvailableAmount { get; set; }
        public double TotalRefundedAmount { get; set; }
    }
}
