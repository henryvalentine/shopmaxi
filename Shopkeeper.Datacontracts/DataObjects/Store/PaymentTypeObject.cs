
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaymentTypeObject
    {
    
        public int PaymentTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<SalePaymentObject> SalePaymentObjects { get; set; }
    }
}
