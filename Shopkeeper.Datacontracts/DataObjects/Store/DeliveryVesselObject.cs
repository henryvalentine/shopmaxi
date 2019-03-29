
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public class DeliveryVesselObject
    {
    
        public long DeliveryVesselId { get; set; }
        public string DeliveryTypeCode { get; set; }
        public string RegistrationCode { get; set; }
        public string Description { get; set; }

        public virtual ICollection<DeliveryObject> DeliverieObjects { get; set; }
    }
}

