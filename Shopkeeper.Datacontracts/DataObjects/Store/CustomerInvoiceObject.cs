
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class CustomerInvoiceObject
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public double TotalAmountPaid { get; set; }
        public double TotalAmountDue { get; set; }
        public double InvoiceBalance { get; set; }
        public double TotalVATAmount { get; set; }
        public double TotalDiscountAmount { get; set; }

        public virtual CustomerObject CustomerObject { get; set; }
    }
}

