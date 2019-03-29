

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    using System;
    using System.Collections.Generic;

    public partial class EstimateObject
    {
        public long Id { get; set; }
        public bool ConvertedToInvoice { get; set; }
        public string EstimateNumber { get; set; }
        public long CreatedById { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<System.DateTime> DateConverted { get; set; }
        public double VAT { get; set; }
        public double Discount { get; set; }
        public long CustomerId { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public int OutletId { get; set; }
        public double AmountDue { get; set; }
        public double NetAmount { get; set; }
        public double VATAmount { get; set; }
        public double DiscountAmount { get; set; }

        public virtual StoreOutletObject StoreOutletObject { get; set; }
        public virtual CustomerObject CustomerObject { get; set; }
        public virtual UserProfileObject UserProfileObject { get; set; }
        public virtual ICollection<EstimateItemObject> EstimateItemObjects { get; set; }
    }

    public partial class EstimateObject
    {
        public string InvoiceStatus { get; set; }
        public string GeneratedByEmployee { get; set; }
        public string DateCreatedStr { get; set; }
        public string LastUpdatedStr { get; set; }
        public int StoreOutletId { get; set; }
        public string OutletName { get; set; }
        public string CustomerName { get; set; }
        public string AmountDueStr { get; set; }

        public string NetAmountStr{ get; set; }
        public string  DiscountAmountStr{ get; set; }
        public string VATAmountStr{ get; set; }
        public long ContactPersonId { get; set; }
    }
}

