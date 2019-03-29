
using System;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class CustomerObject
    {
        public string UserProfileName { get; set; }
        public string CustomerTypeName { get; set; }
        public string StoreOutletName { get; set; }
        public string BirthDayStr { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string FirstPurchaseDateStr { get; set; }
        public string ContactEmail { get; set; }
        public string OfficeLine { get; set; }
        public string Gender { get; set; }
        public double CreditLimit { get; set; }
        public bool CreditWorthy { get; set; }
        public double TotalAmountPaid { get; set; }
        public double TotalAmountDue { get; set; }
        public double TotalNetAmount { get; set; }
        public double InvoiceBalance { get; set; }
        public double TotalVATAmount { get; set; }
        public double TotalDiscountAmount { get; set; }
        public string DateProfiledStr { get; set; }
        public DateTime? Birthday { get; set; }
    }

}
 
