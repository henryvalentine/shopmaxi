
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class ShoppingCartInvoiceObject
    {
        public long Id { get; set; }
        public long ShoppingCartId { get; set; }
        public int PaymentStatus { get; set; }
        public string PaymentReference { get; set; }
        public double AmountDue { get; set; }
        public Nullable<double> AmountPaid { get; set; }
        public int PaymentTypeId { get; set; }
        public Nullable<System.DateTime> DatePaid { get; set; }
        public System.DateTime DateGenerated { get; set; }

        public virtual ShoppingCartObject ShoppingCartObject { get; set; }
    }
}
