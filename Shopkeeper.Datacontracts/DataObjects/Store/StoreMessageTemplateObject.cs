
using Shopkeeper.DataObjects.DataObjects.Store;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class StoreMessageTemplateObject 
    {
        public int Id { get; set; }
        public int EventTypeId { get; set; }
        public string Subject { get; set; }
        public string MessageContent { get; set; }
        public string Footer { get; set; }
    
        public virtual ICollection<StoreMessageObject> MessageObjects { get; set; }
    }
}

