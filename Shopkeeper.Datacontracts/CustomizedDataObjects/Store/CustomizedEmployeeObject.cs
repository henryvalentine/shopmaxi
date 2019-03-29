
using System;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public partial class EmployeeObject
    {
        public string Address { get; set; }
        public string JobTitle { get; set; }
        public string RoleId { get; set; }
        public string AspNetUserId { get; set; }
        public string Name { get; set; }
        public string Outlet { get; set; }
        public long StoreStateId { get; set; }
        public long StoreCountryId { get; set; }  
        public string Department { get; set; }
        public string CityName { get; set; }
        public long StoreCityId { get; set; }
        public string StreetNo { get; set; }
        public string DateHiredStr { get; set; }
        public string DateLeftStr { get; set; }
        public string StatusStr { get; set; }
        public string PhoneNumber { get; set; }
        public string OtherNames { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ConfirmPassword { get; set; }
        public string OriginalPassword { get; set; }
        public string PhotofilePath { get; set; }
        public string BirthdayStr { get; set; }
        public DateTime? Birthday { get; set; }
        public string UserRole { get; set; }
    }

}
