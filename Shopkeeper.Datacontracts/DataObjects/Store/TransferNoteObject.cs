
using System;
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class TransferNoteObject
    {
        public long Id { get; set; }
        public double TotalAmount { get; set; }
        public DateTime DateGenerated { get; set; }
        public long GeneratedByUserId { get; set; }
        public int SourceOutletId { get; set; }
        public int TargetOutletId { get; set; }
        public int Status { get; set; }
        public string TransferNoteNumber { get; set; }
        public DateTime? DateTransferd { get; set; }

        public virtual StoreOutletObject StoreOutletObject { get; set; }
        public virtual StoreOutletObject StoreOutletObject1 { get; set; }
        public virtual UserProfileObject UserProfileObject { get; set; }
        public virtual List<TransferNoteItemObject> TransferNoteItemObjects { get; set; }
    }

    public partial class TransferNoteObject
    {
        public string GeneratedBy { get; set; }
        public string DateTransferdStr { get; set; }
        public string DateGeneratedStr { get; set; }
        public string SourceOutletName { get; set; }
        public string TargetOutletName { get; set; }
        public string TotalAmountStr { get; set; }
        public string StatusStr { get; set; }
    }
}

