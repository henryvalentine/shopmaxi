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
    
    public partial class ItemPrice
    {
        public long ItemPriceId { get; set; }
        public long StoreItemStockId { get; set; }
        public double Price { get; set; }
        public double MinimumQuantity { get; set; }
        public string Remark { get; set; }
        public long UoMId { get; set; }
    
        public virtual StoreItemStock StoreItemStock { get; set; }
        public virtual UnitsOfMeasurement UnitsOfMeasurement { get; set; }
    }
}
