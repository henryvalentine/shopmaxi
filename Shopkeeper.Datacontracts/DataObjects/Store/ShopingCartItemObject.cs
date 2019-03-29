
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class ShopingCartItemObject
    {
        public long ShopingCartItemId { get; set; }
        public long ShopingCartId { get; set; }
        public long StoreItemStockId { get; set; }
        public double UnitPrice { get; set; }
        public double QuantityOrdered { get; set; }
        public long UoMId { get; set; }
        public double Discount { get; set; }
    
        public virtual ShoppingCartObject ShoppingCartObject { get; set; }
        public virtual StoreItemStockObject StoreItemStockObject { get; set; }
    }
}
