
using Newtonsoft.Json;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class InvoiceObject
    {
        public long InvoiceId { get; set; }
        public string ReferenceCode { get; set; }
        public long PurchaseOrderId { get; set; }
        public int StatusCode { get; set; }
        public System.DateTime DueDate { get; set; }
        public System.DateTime DateSent { get; set; }
        public string Attachment { get; set; }
    
        public virtual PurchaseOrderObject PurchaseOrderObject { get; set; }
    }
    
    
}

