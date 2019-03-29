
using System;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class UserProfileObject
    {
        public string BirthdayStr { get; set; }
        public string Salutation { get; set; }
        public string Name { get; set; }
        public int StoreCustomerTypeId { get; set; }
        public int StoreOutletId { get; set; }
        public DeliveryAddressObject DeliveryAddressObject { get; set; }
        public CustomerInvoiceObject CustomerInvoiceObject { get; set; }
        public string DateHiredStr { get; set; }
        public long EmployeeId { get; set; }
        public long? ContactPersonId { get; set; }
        public long StoreCityId { get; set; }
        public DateTime DateHired { get; set; }
        public DateTime? DateLeft { get; set; }
        public DateTime? DateProfiled { get; set; }
        public string StatusStr  { get; set; }
        public string AspNetUserId { get; set; }
        public string DateLeftStr  { get; set; }
        public int Status  { get; set; }
        public string CityName  { get; set; }
        public string Address  { get; set; }
        public string JobTitle  { get; set; }
        public string Outlet  { get; set; }
        public string Department  { get; set; }
        public long StoreDepartmentId  { get; set; }
        public long StoreAddressId  { get; set; }
        public long JobRoleId  { get; set; }
        public string EmployeeNo  { get; set; }
             
    }
}
