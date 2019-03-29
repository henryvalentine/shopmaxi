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
    
    public partial class UnitsOfMeasurement
    {
        public UnitsOfMeasurement()
        {
            this.ItemPrices = new HashSet<ItemPrice>();
            this.TransferNoteItems = new HashSet<TransferNoteItem>();
        }
    
        public long UnitOfMeasurementId { get; set; }
        public string UoMCode { get; set; }
        public string UoMDescription { get; set; }
    
        public virtual ICollection<ItemPrice> ItemPrices { get; set; }
        public virtual ICollection<TransferNoteItem> TransferNoteItems { get; set; }
    }
}