
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class SaleObject
    {
        public string SaleEmployeeName { get; set; }
        public string UoMCode { get; set; }
        public string StoreAddress { get; set; }
        public string StoreName { get; set; }
        public double TotalSales { get; set; }
        public string StoreItemName { get; set; }
        public bool ProcessEstimate { get; set; } 
        public string StoreItem { get; set; }
        public string ItemVariationValue { get; set; }
        public string CustomerName { get; set; }
        public string PaymentStatus { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime DatePaid { get; set; }
        public string DateStr { get; set; }
        public string NetAmountStr { get; set; }
        public string DiscountAmountStr { get; set; } 
        public string AmountDueStr { get; set; }
        public string VATAmountStr { get; set; }
        public string PaymentTypeName { get; set; }
        public string OutletName { get; set; }
        public int OutletId { get; set; }
        public string DateRevokedStr { get; set; }
        public string BalanceStr { get; set; }
        public double AmountRefunded { get; set; }
        public string StatusStr { get; set; }
        public double AmountPaid { get; set; }
        public double Balance { get; set; }
        public string AmountPaidStr { get; set; }
        public long StoreTransactionId { get; set; }
        public long StoreItemStockId { get; set; }
        public string RegisterName { get; set; }
        public List<StoreTransactionObject> Transactions { get; set; }
        public List<SalePaymentObject> Payments { get; set; }
        public List<StoreItemSoldObject> SoldItems { get; set; }
        public int NumberSoldItems { get; set; }
    }
}

