using System.ComponentModel.DataAnnotations;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public class RegisterModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ReferenceCode { get; set; }
        public string CompanyName { get; set; }
        public int Duration { get; set; }
        public bool IsTrial { get; set; }
        public bool IsBankOption { get; set; }
        public string PackageName { get; set; }
        public string StoreName { get; set; }
        public string PaymentOption { get; set; }
        public double AmountDue { get; set; }
        public long Gx { get; set; }
        public string Tbsr { get; set; }
    }
}
