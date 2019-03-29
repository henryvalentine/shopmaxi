
namespace Shopkeeper.DataObjects.DataObjects.Store
{

    public class EmployeeSalesLogObject
    {
        public long EmployeeSalesLogId { get; set; }
        public long EmployeeId { get; set; }
        public double TotalSales { get; set; }
        public System.DateTime SalesDate { get; set; }

        public virtual EmployeeObject EmployeeObject { get; set; }
    }
}
