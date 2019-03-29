using System.Collections.Generic;
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreMessageObject
    {
        public string EventTypeName { get; set; }
        public string Receipient { get; set; }
        public int EventTypeId { get; set; }
        public string Subject { get; set; }
        public string MessageContent { get; set; }
        public string Footer { get; set; }
        public string DateSentStr { get; set; }
        public string StatusStr { get; set; }
    }
}
         