
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.DataObjects.DataObjects.Store;
using UserProfileObject = Shopkeeper.DataObjects.DataObjects.Store.UserProfileObject;

namespace Shopkeeper.Datacontracts.CustomizedDataObjects
{
    public class GenericObject
    {
        public List<StoreItemBrandObject> ProductBrands { get; set; }
        public List<StoreItemObject> Products { get; set; }
        public List<StoreItemTypeObject> ProductTypes { get; set; }
        public List<StoreItemCategoryObject> ProductCategories { get; set; }
        public List<ChartOfAccountObject> ChartsOfAccount { get; set; }

    }

    public class StockGenericObject
    {
        public List<StoreOutletObject> StoreOutlets { get; set; }
        public List<StoreStateObject> States { get; set; }
        public List<StoreItemVariationObject> StoreItemVariations { get; set; }
        public List<StoreItemVariationValueObject> StoreItemVariationValues { get; set; }
        public List<StoreItemObject> StoreItems { get; set; }
        public List<StoreItemStockObject> Inventories { get; set; }
        public List<StorePaymentMethodObject> PaymentMethods { get; set; }
        public List<StoreCurrencyObject> StoreCurrencies { get; set; }
        public List<UnitsOfMeasurementObject> UnitsofMeasurement { get; set; }
        public List<ImageViewObject> ImageViews { get; set; }
        public List<StoreItemCategoryObject> Categories { get; set; }
        public List<CustomerObject> Customers { get; set; }
        public StoreCurrencyObject Currency { get; set; }
        public long Dbx { get; set; }
        public List<StoreCustomerTypeObject> CustomerTypes { get; set; }
    }

    public class CustomerGenericObject
    {
        public List<CustomerObject> Customers { get; set; }
        public List<StoreCountryObject> Countries { get; set; }
        public List<StateObject> States { get; set; }
        public List<StoreCustomerTypeObject> CustomerTypes { get; set; }
        public List<StoreOutletObject> StoreOutlets { get; set; }
        public List<UserProfileObject> UserProfiles { get; set; }
        public List<EmployeeObject> Employees { get; set; }
    }

    public class StoreGenericObject
    {
        public List<CurrencyObject> Currencies { get; set; }
        public List<BillingCycleObject> BillingCycles { get; set; }
        public List<PaymentMethodObject> PaymentMethods { get; set; }
    }

    public class EmployeeGenericObject
    {
        public List<StoreDepartmentObject> Departments { get; set; }
        public List<UserProfileObject> UserProfiles { get; set; }
        public List<JobRoleObject> JobRoles { get; set; }
        public List<StoreCityObject> Cities { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }

    public class DBObject
    {
        public string ConnectionString { get; set; }
        public long DBSize { get; set; }
        public string DBName { get; set; }
        public string ScriptFilePath { get; set; }

    }

}

