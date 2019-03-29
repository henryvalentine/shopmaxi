//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopkeeperStore.EF.Models.Store
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeAssigment
    {
        public long EmployeeAssignmentId { get; set; }
        public long EmployeeId { get; set; }
        public long JobRoleId { get; set; }
        public int StoreOutletId { get; set; }
        public System.DateTime FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public byte[] LastUpdate { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual JobRole JobRole { get; set; }
        public virtual StoreOutlet StoreOutlet { get; set; }
    }
}