using System;
using System.Data.Entity;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Infrastructures.ShopkeeperInfrastructures
{
    public class ShopkeeperStoreContext : DbContext, IShopkeeperContext
    {
        public ShopkeeperStoreContext(string contextConnector) : base(contextConnector)
        {
            ShopkeeperContext = this;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

         public ShopkeeperStoreContext(DbContext context) : base(context.Database.Connection.ConnectionString)
        {
            ShopkeeperContext = context;
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

         public ShopkeeperStoreContext()
         {
             ShopkeeperContext = this;
             Configuration.LazyLoadingEnabled = false;
             Configuration.ProxyCreationEnabled = false;
         }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DailyInventory> DailyInventories { get; set; }
        public virtual DbSet<DailySale> DailySales { get; set; }
        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<DeliveryVessel> DeliveryVessels { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<EmployeeAssigment> EmployeeAssigments { get; set; }
        public virtual DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
        public virtual DbSet<EmployeeSalesLog> EmployeeSalesLogs { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<JobRole> JobRoles { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderPayment> OrderPayments { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<PaymentOption> PaymentOptions { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<PersonPhoneContact> PersonPhoneContacts { get; set; }
        public virtual DbSet<ProductStock> ProductStocks { get; set; }
        public virtual DbSet<ProductSupplier> ProductSuppliers { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public virtual DbSet<StoreCustomerType> StoreCustomerTypes { get; set; }
        public virtual DbSet<StoreDepartment> StoreDepartments { get; set; }
        public virtual DbSet<StoreOutlet> StoreOutlets { get; set; }
        public virtual DbSet<StorePaymentMethod> StorePaymentMethods { get; set; }
        public virtual DbSet<StoreProduct> StoreProducts { get; set; }
        public virtual DbSet<StoreProductBrand> StoreProductBrands { get; set; }
        public virtual DbSet<StoreProductCategory> StoreProductCategories { get; set; }
        public virtual DbSet<StoreProductType> StoreProductTypes { get; set; }
        public virtual DbSet<StoreTransaction> StoreTransactions { get; set; }
        public virtual DbSet<StoreTransactionType> StoreTransactionTypes { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<WayBill> WayBills { get; set; }
        public virtual DbSet<WayBillItem> WayBillItems { get; set; }

        void IDisposable.Dispose()
        {
            ShopkeeperContext.Dispose();
        }
        public DbContext ShopkeeperContext { get; private set; }
    }
}
