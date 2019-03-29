using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Shopkeeper.DataObjects.DataObjects.Store;

namespace ShopKeeper.GenericHelpers
{
    public class InvoiceItem
    {
          public  long PurchaseOrderId { get; set; }
          public long InvoiceId  { get; set; }
          public string InitialFilePath  { get; set; }
          public DateTime DateReceived  { get; set; }
          public string Code { get; set; }

    }
}
