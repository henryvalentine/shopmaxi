
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    using System.Collections.Generic;
    
    public partial class CountryObject
    {
        public long CountryId { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<CurrencyObject> CurrencyObjects { get; set;}
        public virtual ICollection<StateObject> StateObjects { get; set; }
    }
}
