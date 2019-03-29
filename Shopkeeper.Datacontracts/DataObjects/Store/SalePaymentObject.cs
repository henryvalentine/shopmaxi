
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class SalePaymentObject
    {
        public long SalePaymentId { get; set; }
        public long SaleId { get; set; }
        public double AmountPaid { get; set; }
        public System.DateTime DatePaid { get; set; }

        public virtual SaleObject SaleObject { get; set; }
    }
}
