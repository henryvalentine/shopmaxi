
namespace Shopkeeper.DataObjects.DataObjects.Master
{
    public partial class CurrencyObject
    {
        public long CurrencyId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public long CountryId { get; set; }
    
        public virtual CountryObject CountryObject { get; set; }
    }
}
