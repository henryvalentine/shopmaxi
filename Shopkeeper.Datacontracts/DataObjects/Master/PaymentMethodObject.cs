
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;
    
    public partial class PaymentMethodObject
    {
    
        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TransactionObject> TransactionObjects { get; set; }
    }
}
