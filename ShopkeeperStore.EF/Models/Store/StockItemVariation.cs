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
    
    public partial class StockItemVariation
    {
        public long Id { get; set; }
        public long StoreItemStockId { get; set; }
        public int VariationId { get; set; }
        public int VariationValueId { get; set; }
        public string SampleImagePath { get; set; }
    
        public virtual StoreItemStock StoreItemStock { get; set; }
        public virtual StoreItemVariation StoreItemVariation { get; set; }
        public virtual StoreItemVariationValue StoreItemVariationValue { get; set; }
    }
}