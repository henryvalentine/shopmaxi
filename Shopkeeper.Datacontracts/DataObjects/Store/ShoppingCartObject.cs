
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class ShoppingCartObject
    {

        public long ShoppingCartId { get; set; }
        public Nullable<long> CustomerId { get; set; }
        public int DeliveryStatus { get; set; }
        public string CustomerIpAddress { get; set; }
        public Nullable<long> DeliveryAddressId { get; set; }
        public System.DateTime DateInitiated { get; set; }
        public Nullable<System.DateTime> DateDelivered { get; set; }

        public virtual CouponObject CouponObject { get; set; }
        public virtual CustomerObject CustomerObject { get; set; }
        public virtual DeliveryAddressObject DeliveryAddressObject { get; set; }
        public virtual List<ShopingCartItemObject> ShopingCartItemObjects { get; set; }
        public virtual List<ShoppingCartDeliveryObject> ShoppingCartDeliveryObjects { get; set; }
        public virtual List<ShoppingCartInvoiceObject> ShoppingCartInvoiceObjects { get; set; }
    }
}
