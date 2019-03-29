
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System.Collections.Generic;
    
    public  class PaymentOptionObjects
    {
    
        public int PaymentOptionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<OrderPaymentObject> OrderPaymentObjects { get; set; }
    }
}
