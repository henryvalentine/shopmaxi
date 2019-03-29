
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class StoreItemIssueObject
    {
        public long StoreItemIssueId { get; set; }
        public long StoreItemStockId { get; set; }
        public int IssueTypeId { get; set; }
        public string Reason { get; set; }
        public System.DateTime IssueDate { get; set; }
        public int ResolutionStatus { get; set; }
    
        public virtual IssueTypeObject IssueTypeObject { get; set; }
        public virtual StoreItemStockObject StoreItemStockObject { get; set; }
    }
}
