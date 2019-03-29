
namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public class EmployeeDocumentObject
    {
        public long EmployeeDocumentId { get; set; }
        public long EmployeeId { get; set; }
        public int DocumentTypeId { get; set; }
        public string FilePath { get; set; }
        public byte[] DateAttached { get; set; }

        public virtual EmployeeObject EmployeeObject { get; set; }
    }
}
