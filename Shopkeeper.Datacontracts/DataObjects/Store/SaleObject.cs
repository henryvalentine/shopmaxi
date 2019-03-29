
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class SaleObject
    {
        public long SaleId { get; set; }
        public double AmountDue { get; set; }
        public int Status { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<int> RegisterId { get; set; }
        public long EmployeeId { get; set; }
        public Nullable<long> CustomerId { get; set; }
        public double Discount { get; set; }
        public double VAT { get; set; }
        public double NetAmount { get; set; }
        public string InvoiceNumber { get; set; }
        public double VATAmount { get; set; }
        public double DiscountAmount { get; set; }
        public string EstimateNumber { get; set; }

        public virtual CustomerObject CustomerObject { get; set; }
        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual ICollection<InvoiceRequestObject> InvoiceRequestObjects { get; set; }
        public virtual RegisterObject RegisterObject { get; set; }
        public virtual List<RefundNoteObject> RefundNoteObjects { get; set; }
        public virtual ICollection<SalePaymentObject> SalePaymentObjects { get; set; }
        public virtual ICollection<SaleTransactionObject> SaleTransactionObjects { get; set; }
        public virtual ICollection<StoreItemSoldObject> StoreItemSoldObjects { get; set; }
    }

}

