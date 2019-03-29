
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class CustomerInvoiceObject
    {
        public double AmountDue { get; set; }
        public double AmountPaid { get; set; }
        public string DateProfiledStr{get; set; }
        public string TotalAmountPaidStr{get; set; }
        public string TotalVATAmountStr {get; set; }
        public string TotalDiscountAmountStr{get; set; }
        public string OutletName { get; set; }
        public string TotalAmountDueStr{get; set; }
        public string InvoiceBalanceStr { get; set; }
        public string CustomerName { get; set; }
        public DateTime? DateProfiled { get; set; }
    }
    
    public class SupplierInvoiceObject
    {
        public long SupplierId { get; set; }
        public double TotalAmountPaid { get; set; }
        public double TotalAmountDue { get; set; }
        public double InvoiceBalance { get; set; }
        public double TotalVATAmount { get; set; }
        public double TotalDiscountAmount { get; set; }
        public DateTime DateJoined { get; set; }
        public string SupplierName { get; set; }
        public string DateJoinedStr { get; set; }
        public string AmountDueStr{ get; set; }
        public string VATAmountStr{ get; set; }
        public string NetAmountStr{ get; set; }
        public string DiscountAmountStr { get; set; }
        public string BalanceStr { get; set; }
        public string TotalAmountPaidStr { get; set; }
    }
}


