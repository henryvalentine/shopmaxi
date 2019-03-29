
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System.Collections.Generic;
    
    public partial class PaymentGatewayObject
    {
        public long PaymentGatewayId { get; set; }
        public string GatewayName { get; set; }
        public string LogoPath { get; set; }
    
        public virtual ICollection<StorePaymentGatewayObject> StorePaymentGatewayObjects { get; set; }
    }
}
