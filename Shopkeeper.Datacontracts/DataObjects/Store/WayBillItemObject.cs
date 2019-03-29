namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public  class WayBillItemObject
    {
        public long WayBillItemId { get; set; }
        public long WayBillId { get; set; }
        public long StoreItemId { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }
    
        public virtual StoreItemObject StoreItemObject { get; set; }
        public virtual WayBillObject WayBillObject { get; set; }
    }
}
