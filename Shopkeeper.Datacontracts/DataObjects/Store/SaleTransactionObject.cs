
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class SaleTransactionObject
    {
        public long SaleTransactionId { get; set; }
        public long SaleId { get; set; }
        public long StoreTransactionId { get; set; }

        public virtual SaleObject SaleObject { get; set; }
        public virtual StoreTransactionObject StoreTransactionObject { get; set; }
    }
}
