//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopkeeperStore.EF.Models.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class ShopingCartItem
    {
        public long ShopingCartItemId { get; set; }
        public long ShopingCartId { get; set; }
        public long StoreItemStockId { get; set; }
        public double UnitPrice { get; set; }
        public double QuantityOrdered { get; set; }
        public long UoMId { get; set; }
        public double Discount { get; set; }
    
        public virtual ShoppingCart ShoppingCart { get; set; }
        public virtual StoreItemStock StoreItemStock { get; set; }
    }
}
