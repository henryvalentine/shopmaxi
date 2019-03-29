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
    
    public partial class UserProfile
    {
        public UserProfile()
        {
            this.AspNetUsers = new HashSet<AspNetUser>();
            this.Customers = new HashSet<Customer>();
            this.Employees = new HashSet<Employee>();
            this.Employees1 = new HashSet<Employee>();
            this.Estimates = new HashSet<Estimate>();
            this.InvoiceRequests = new HashSet<InvoiceRequest>();
            this.PersonContacts = new HashSet<PersonContact>();
            this.StoreContacts = new HashSet<StoreContact>();
            this.StoreMessages = new HashSet<StoreMessage>();
            this.TransferNotes = new HashSet<TransferNote>();
        }
    
        public long Id { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string PhotofilePath { get; set; }
        public bool IsActive { get; set; }
        public string ContactEmail { get; set; }
        public string MobileNumber { get; set; }
        public string OfficeLine { get; set; }
    
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Employee> Employees1 { get; set; }
        public virtual ICollection<Estimate> Estimates { get; set; }
        public virtual ICollection<InvoiceRequest> InvoiceRequests { get; set; }
        public virtual ICollection<PersonContact> PersonContacts { get; set; }
        public virtual ICollection<StoreContact> StoreContacts { get; set; }
        public virtual ICollection<StoreMessage> StoreMessages { get; set; }
        public virtual ICollection<TransferNote> TransferNotes { get; set; }
    }
}
