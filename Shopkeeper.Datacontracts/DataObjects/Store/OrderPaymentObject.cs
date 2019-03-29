
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public  class OrderPaymentObject
    {
        public long OrderPaymentId { get; set; }
        public long OrderId { get; set; }
        public double AmountPaid { get; set; }
        public int PaymentOptionId { get; set; }
        public long EmployeeObjectId { get; set; }
        public byte[] DatePaid { get; set; }
    
        public virtual EmployeeObject EmployeeObject { get; set; }
        public virtual OrderObject OrderObject { get; set; }
        public virtual PaymentOptionObjects PaymentOptionObjects { get; set; }
    }
}
