
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public  class PurchaseObject
    {
        public long PurchaseId { get; set; }
        public string ExpenseType { get; set; }
        public Nullable<double> AmountPaid { get; set; }
        public long SupplierId { get; set; }
        public System.DateTime DateMade { get; set; }
        public string InvoiceFilePath { get; set; }
    
        public virtual SupplierObject SupplierObject { get; set; }
    }
}
