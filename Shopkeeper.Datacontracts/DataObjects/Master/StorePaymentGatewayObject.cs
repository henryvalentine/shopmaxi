
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class StorePaymentGatewayObject
    {
        public long StorePaymentgatewayId { get; set; }
        public long PaymentGatewayId { get; set; }
        public long StoreId { get; set; }
        public string MerchantId { get; set; }

        public virtual PaymentGatewayObject PaymentGatewayObject { get; set; }
        public virtual StoreObject StoreObject { get; set; }
    }
}
