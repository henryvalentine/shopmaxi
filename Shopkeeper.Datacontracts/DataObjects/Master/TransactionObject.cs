
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    
    public partial class TransactionObject
    {
        public long TransactionId { get; set; }
        public int TransactionTypeId { get; set; }
        public int PaymentMethodId { get; set; }
        public double Amount { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string Remark { get; set; }
    
        public virtual PaymentMethodObject PaymentMethodObject { get; set; }
        public virtual TransactionTypeObject TransactionType { get; set; }
    }
}
