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
    
    public partial class StoreMessageTemplate
    {
        public StoreMessageTemplate()
        {
            this.StoreMessages = new HashSet<StoreMessage>();
        }
    
        public int Id { get; set; }
        public int EventTypeId { get; set; }
        public string Subject { get; set; }
        public string MessageContent { get; set; }
        public string Footer { get; set; }
    
        public virtual ICollection<StoreMessage> StoreMessages { get; set; }
    }
}