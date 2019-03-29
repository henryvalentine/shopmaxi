namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public class ShoppingCartDeliveryObject
    {
        public long Id { get; set; }
        public long ShoppingCartId { get; set; }
        public long DeliveredById { get; set; }
        public int DelievryMethodId { get; set; }
        public System.DateTime ExpectedDeliveryDate { get; set; }
        public System.DateTime ActualDateDelivered { get; set; }
        public Nullable<long> DeliveryVesselId { get; set; }

        public virtual DeliveryMethodObject DeliveryMethodObject { get; set; }
        public virtual DeliveryVesselObject DeliveryVesselObject { get; set; }
        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual ShoppingCartObject ShoppingCartObject { get; set; }
    }
}
