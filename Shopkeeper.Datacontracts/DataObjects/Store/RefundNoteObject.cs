
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class RefundNoteObject
    {
        public long Id { get; set; }
        public long SaleId { get; set; }
        public double Discount { get; set; }
        public double DiscountAmount { get; set; }
        public double VAT { get; set; }
        public double VATAmount { get; set; }
        public int PaymentMethodId { get; set; }
        public System.DateTime DateReturned { get; set; }
        public double AmountDue { get; set; }
        public double NetAmount { get; set; }
        public Nullable<int> IssueTypeId { get; set; }
        public long EmployeeId { get; set; }
        public long CustomerId { get; set; }
        public string RefundNoteNumber { get; set; }
        public long TransactionId { get; set; }

        public virtual CustomerObject CustomerObject { get; set; }
        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual IssueTypeObject IssueTypeObject { get; set; }
        public virtual SaleObject SaleObject { get; set; }
        public virtual StorePaymentMethodObject StorePaymentMethodObject { get; set; }
        public virtual StoreTransactionObject StoreTransactionObject { get; set; }
        public virtual List<ReturnedProductObject> ReturnedProductObjects { get; set; }
    }

    public partial class RefundNoteObject
    {
        public string DiscountAmountStr { get; set; }
        public string VATAmountStr { get; set; }
        public string PaymentMethodName { get; set; }
        public int OutletId { get; set; }
        public string DateReturnedStr { get; set; }
        public string AmountDueStr { get; set; }
        public string NetAmountStr { get; set; }
        public string IssueTypeName { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string Reason { get; set; }
        public string InvoiceNumber { get; set; }
        public double TotalAmountRefunded { get; set; }
    }
}

