
using System.Collections.Generic;

namespace Shopkeeper.DataObjects.DataObjects.Store
{
    public class GenericSaleObject
    {
        public SaleObject Sale { get; set; }
        public List<StoreTransactionObject> StoreTransactions { get; set; }
        public List<StoreItemSoldObject> SoldItems { get; set; }
    }

    public class PurchaseOrderSelectable
    {
        public List<ChartOfAccountObject> ChartOfAccounts { get; set; }
        public List<StoreOutletObject> StoreOutlets { get; set; }
        public List<SupplierObject> Suppliers { get; set; }

    }
}

