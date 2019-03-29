
using Newtonsoft.Json;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class InvoiceObject
    {
        public string DateSentStr { get; set; }
       
    }
    
    public class InvoiceJson
    {
        [JsonProperty("InvoiceId")]
        public long InvoiceId { get; set; }

        [JsonProperty("ReferenceCode")]
        public string ReferenceCode { get; set; }

        [JsonProperty("PurchaseOrderId")]
        public long PurchaseOrderId { get; set; }

        [JsonProperty("StatusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("DueDate")]
        public System.DateTime DueDate { get; set; }

        [JsonProperty("DateSent")]
        public System.DateTime DateSent { get; set; }

        [JsonProperty("Attachment")]
        public string Attachment { get; set; }

        [JsonProperty("PurchaseOrderObject")]
        public virtual PurchaseOrderObject PurchaseOrderObject { get; set; }
    }
}

