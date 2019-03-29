

namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class StoreBankAccountObject
    {
        public long StoreBankAccountId { get; set; }
        public long StoreId { get; set; }
        public long BankId { get; set; }
        public string AccountName { get; set; }
        public long AccountNo { get; set; }
        public bool Status { get; set; }
    
        public virtual BankObject BankObject { get; set; }
        public virtual StoreObject StoreObject { get; set; }
    }
}
