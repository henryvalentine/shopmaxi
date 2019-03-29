
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System;
    using System.Collections.Generic;
    
    public partial class BankObject
    {
        public long BankId { get; set; }
        public string SortCode { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string LogoPath { get; set; }
        public string LastUpdated { get; set; }
    
        public virtual ICollection<StoreBankAccountObject> StoreBankAccountObjects { get; set; }
    }
}
