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
    
    public partial class StockUpload
    {
        public long StockUploadId { get; set; }
        public long StoreItemStockId { get; set; }
        public int ImageViewId { get; set; }
        public string ImagePath { get; set; }
        public System.DateTime LastUpdated { get; set; }
    
        public virtual ImageView ImageView { get; set; }
        public virtual StoreItemStock StoreItemStock { get; set; }
    }
}