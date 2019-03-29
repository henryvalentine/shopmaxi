
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class BankAccountObject
    {
        public long BankAccountId { get; set; }
        public long BankId { get; set; }
        public string AccountName { get; set; }
        public long AccountNo { get; set; }
        public bool Status { get; set; }
        public long CustomerId { get; set; }

        public virtual BankObject BankObject { get; set; }
        public virtual CustomerObject CustomerObject { get; set; }
    }
}
